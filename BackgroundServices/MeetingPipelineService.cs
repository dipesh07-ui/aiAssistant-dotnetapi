using aiAssistant.api.DTOs;
using aiAssistant.api.Models;
using aiAssistant.api.Repositories.Interfaces;
using aiAssistant.api.Services;
using aiAssistant.api.Services.Interfaces;
using Hangfire;
using System.Text.Json;

namespace aiAssistant.api.BackgroundServices
{
    public class MeetingPipelineService(
     INlpService _nlpService,
     IMeetingRepository _meetingRepo,
     SseStreamService _sse,
     ILogger<MeetingPipelineService> _logger)
    {
        [AutomaticRetry(Attempts = 2)]
        public async Task RunPipelineAsync(PipelineJob job)
        {
            _sse.Create(job.MeetingId);

            try
            {
                await Push(job.MeetingId, "processing",
                    "Downloading and transcribing...", 10);

                // ONE call does everything now
                var result = await _nlpService.FullProcessAsync(
                    job.Source, job.SourceType);

                if (!result.IsSuccess)
                {
                    await Fail(job.MeetingId, result.Error!);
                    return;
                }

                await Push(job.MeetingId, "saving", "Saving to database...", 90);
                await SaveMeeting(job.MeetingId, result.Value!);

                await Push(job.MeetingId, "done", "Ready!", 100);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pipeline crashed for {MeetingId}", job.MeetingId);
                await Fail(job.MeetingId, ex.Message);
                throw;
            }
            finally
            {
                _sse.Close(job.MeetingId);
            }
        }

        private async Task SaveMeeting(Guid meetingId, FullProcessResult r)
        {
            var meeting = await _meetingRepo.GetByIdInternalAsync(meetingId);
            if (meeting == null) return;

            meeting.Transcript = r.Transcript;
            meeting.Title = r.Title;
            meeting.Summary = r.Summary;
            meeting.ActionItems = JsonSerializer.Serialize(r.ActionItems);
            meeting.Decisions = JsonSerializer.Serialize(r.Decisions);
            meeting.KeyQuestions = JsonSerializer.Serialize(r.KeyQuestions);
            meeting.Status = "ready";
            meeting.UpdatedAt = DateTime.UtcNow;

            await _meetingRepo.UpdateAsync(meeting);
        }

        private async Task Fail(Guid meetingId, string error)
        {
            var meeting = await _meetingRepo.GetByIdInternalAsync(meetingId);
            if (meeting != null)
            {
                meeting.Status = "failed";
                meeting.ErrorMessage = error;
                meeting.UpdatedAt = DateTime.UtcNow;
                await _meetingRepo.UpdateAsync(meeting);
            }
            await Push(meetingId, "failed", error, 0);
        }

        private Task Push(Guid meetingId, string step, string message, int percent) =>
            _sse.PushAsync(meetingId, new ProgressEvent
            {
                Step = step,
                Message = message,
                Percent = percent
            });
    }
}
