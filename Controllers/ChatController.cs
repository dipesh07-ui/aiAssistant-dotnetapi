using aiAssistant.api.Common;
using aiAssistant.api.DTOs;
using aiAssistant.api.Extensions;
using aiAssistant.api.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace aiAssistant.api.Controllers
{
    [Route("api/meetings/{meetingId}/[controller]")]
    [ApiController]
    public class ChatController(
        IChatService _chatService,
        IValidator<ChatRequest> _chatValidator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Ask(Guid meetingId,ChatRequest request)
        {
            var validationResult = await _chatValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(ApiResponse<ChatMessageDto>.Fail(validationResult.Errors.First().ErrorMessage));
            }

           var result = await _chatService.AskMessageAsync(meetingId,this.GetUserId(), request);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<ChatMessageDto>.Fail(result.Error!));
            }

            return Ok(ApiResponse<ChatMessageDto>.Ok(result.Value!));
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory(Guid meetingId)
        {
            var result = await _chatService.ChatHistoryAsync(
                meetingId, this.GetUserId());

            if (!result.IsSuccess)
                return NotFound(ApiResponse<List<ChatMessageDto>>
                    .Fail(result.Error!));

            return Ok(ApiResponse<List<ChatMessageDto>>.Ok(result.Value!));
        }
    }
}
