using aiAssistant.api.Models;

namespace aiAssistant.api.Repositories.Interfaces
{
    public interface IChatRepository
    {
        Task<IEnumerable<ChatMessage>> GetByMeetingIdAsync(Guid meetingId, Guid userId);
        Task<ChatMessage> CreateMessageAsync(ChatMessage chatMessage);
    }
}
