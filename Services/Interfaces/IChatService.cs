using aiAssistant.api.Common;
using aiAssistant.api.DTOs;

namespace aiAssistant.api.Services.Interfaces
{
    public interface IChatService
    {
        Task<Result<List<ChatMessageDto>>> ChatHistoryAsync(Guid meetingId, Guid userId);

        Task<Result<ChatMessageDto>> AskMessageAsync(Guid meetingId, Guid userId, ChatRequest message);

    }
}
