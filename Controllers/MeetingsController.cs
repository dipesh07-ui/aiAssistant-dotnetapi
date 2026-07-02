using aiAssistant.api.BackgroundServices;
using aiAssistant.api.Common;
using aiAssistant.api.DTOs;
using aiAssistant.api.Extensions;
using aiAssistant.api.Models;
using aiAssistant.api.Services;
using aiAssistant.api.Services.Interfaces;
using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace aiAssistant.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController(IMeetingService _meetingService,
        SseStreamService _sseService,
        IValidator<CreateYoutubeRequest> _youtubeValidator
        ) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _meetingService.GetAllAsync(this.GetUserId(), page, pageSize);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<PagedResult<MeetingListItemDto>>.Fail(result.Error!));
            }

            return Ok(ApiResponse<PagedResult<MeetingListItemDto>>.Ok(result.Value!));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _meetingService.GetByIdAsync(id, this.GetUserId());
            if (!result.IsSuccess)
            {
                return NotFound(ApiResponse<MeetingDetailDto>.Fail(result.Error!));
            }
            return Ok(ApiResponse<MeetingDetailDto>.Ok(result.Value!));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _meetingService.DeleteAsync(id, this.GetUserId());
            if (!result.IsSuccess)
            {
                return NotFound(ApiResponse<object>.Fail(result.Error!));
            }
            return Ok(ApiResponse<object>.Ok(new { deleted = true }));
        }

        [HttpPost("youtube")]
        public async Task<IActionResult> CreateYoutubeMeeting(CreateYoutubeRequest request)
        {
            var validationResult = await _youtubeValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(ApiResponse<SubmitResponse>.Fail(validationResult.Errors.First().ErrorMessage));
            }

            var result = await _meetingService.SubmitYoutubeAsync(this.GetUserId(), request);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<SubmitResponse>.Fail(result.Error!));
            }

            BackgroundJob.Enqueue<MeetingPipelineService>(
            s => s.RunPipelineAsync(new PipelineJob
            {
                MeetingId = result.Value!.MeetingId,
                Source = request.Url,
                SourceType = "youtube",
                UserId = this.GetUserId()
            })
        );

            return Accepted(ApiResponse<SubmitResponse>.Ok(result.Value!));

        }

        [HttpPost("upload")]
        [RequestSizeLimit(524_288_000)] // 500MB
        public async Task<IActionResult> SubmitFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<SubmitResponse>
                    .Fail("No file provided"));

            var allowed = new[] { ".mp3", ".wav", ".mp4", ".m4a", ".mov" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowed.Contains(ext))
                return BadRequest(ApiResponse<SubmitResponse>
                    .Fail("File type not supported"));

            var result = await _meetingService.SubmitFileAsync(
                this.GetUserId(), file);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse<SubmitResponse>.Fail(result.Error!));

            // enqueue Hangfire job
            BackgroundJob.Enqueue<MeetingPipelineService>(
                s => s.RunPipelineAsync(new PipelineJob
                {
                    MeetingId = result.Value!.MeetingId,
                    Source = result.Value!.TempFilePath,
                    SourceType = "file",
                    UserId = this.GetUserId()
                })
            );

            return Accepted(ApiResponse<SubmitResponse>.Ok(result.Value!));
        }

        [HttpGet("{id}/stream")]
        public async Task Stream(Guid id, CancellationToken ct)
        {
            // verify meeting belongs to user
            var meeting = await _meetingService.GetByIdAsync(id, this.GetUserId());
            if (!meeting.IsSuccess)
            {
                Response.StatusCode = 404;
                return;
            }

            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["X-Accel-Buffering"] = "no";
            Response.Headers["Connection"] = "keep-alive";

            try
            {
                await foreach (var evt in _sseService
                    .Subscribe(id, ct)
                    .WithCancellation(ct))
                {
                    var data = JsonSerializer.Serialize(evt,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                    await Response.WriteAsync(
                        $"event: progress\ndata: {data}\n\n", ct);
                    await Response.Body.FlushAsync(ct);
                }

                // stream closed → job done
                await Response.WriteAsync(
                    $"event: complete\ndata: {{\"meetingId\":\"{id}\"}}\n\n", ct);
                await Response.Body.FlushAsync(ct);
            }
            catch (OperationCanceledException)
            {
                // user closed browser tab → normal, ignore
            }
        }
    }
}
