using aiAssistant.api.Data;
using aiAssistant.api.Models;
using aiAssistant.api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace aiAssistant.api.Repositories
{
    public class ChatRepository(AppDbContext _db) : IChatRepository
    {
        public async Task<ChatMessage> CreateMessageAsync(ChatMessage chatMessage)
        {
            _db.Add(chatMessage);
            await _db.SaveChangesAsync();
            return chatMessage;
        }

        public async Task<IEnumerable<ChatMessage>> GetByMeetingIdAsync(Guid meetingId, Guid userId)
        {
            return await _db.ChatMessages
                .Where(x=>x.MeetingId==meetingId&& x.Meeting.UserId == userId)
                .OrderBy(x=>x.CreatedAt)
                .ToListAsync();
                
        }
    }
}
