using aiAssistant.api.Common;
using aiAssistant.api.DTOs;
using aiAssistant.api.Extensions;
using aiAssistant.api.Models;
using aiAssistant.api.Repositories.Interfaces;
using aiAssistant.api.Services.Interfaces;

namespace aiAssistant.api.Services
{
    public class ChatService(IChatRepository _chatRepo,IMeetingRepository _meetingRepo,INlpService _nlpService) : IChatService
    {
        public async Task<Result<ChatMessageDto>> AskMessageAsync(Guid meetingId, Guid userId, ChatRequest request)
        {
            var meeting = await _meetingRepo.GetByIdAsync(meetingId, userId);
            if (meeting == null)
            {
                return Result<ChatMessageDto>.Failure("Meeting not found");
            }
            if(meeting.Status != "ready")
            {
                return Result<ChatMessageDto>.Failure("Meeting still processing");
            }

            var chatMessage = new ChatMessage
            {
                Id = Guid.NewGuid(),
                MeetingId = meetingId,
                Role = "user",
                Content = request.Question,
                CreatedAt = DateTime.UtcNow
            };
             await _chatRepo.CreateMessageAsync(chatMessage);
            var nlpResponse = await _nlpService.AskAsync(meeting.Transcript!, request.Question);
            if (!nlpResponse.IsSuccess)
            {
                return Result<ChatMessageDto>.Failure(nlpResponse.Error!);
            }

            var assitantMessage = new ChatMessage
            {
                Id = Guid.NewGuid(),
                MeetingId = meetingId,
                Role = "assistant",
                Content = nlpResponse.Value!,
                CreatedAt = DateTime.UtcNow
            };

            await _chatRepo.CreateMessageAsync(assitantMessage);

            return Result<ChatMessageDto>.Success(assitantMessage.ToDto());

        }

        public async Task<Result<List<ChatMessageDto>>> ChatHistoryAsync(Guid meetingId, Guid userId)
        {
           var meeting = await  _meetingRepo.GetByIdAsync(meetingId, userId);
            if (meeting == null)
            {
                return Result<List<ChatMessageDto>>.Failure("Meeting not found");
            }

            var history = await _chatRepo.GetByMeetingIdAsync(meetingId, userId);
            return Result<List<ChatMessageDto>>.Success(history.Select(m => m.ToDto()).ToList());

        }
    }
}
