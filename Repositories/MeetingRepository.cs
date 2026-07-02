using aiAssistant.api.Data;
using aiAssistant.api.Models;
using aiAssistant.api.Repositories.Interfaces;
using Google.Apis.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace aiAssistant.api.Repositories
{
    public class MeetingRepository(AppDbContext _db) : IMeetingRepository
    {
       
        public async Task<Meeting> CreateAsync(Meeting meeting)
        {
            _db.Add(meeting);
            await _db.SaveChangesAsync();
            return meeting;
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var meeting = await _db.Meetings.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if(meeting !=null)
            {
                _db.Meetings.Remove(meeting);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Meeting>> GetAllAsync(Guid UserId)
        {
           return await _db.Meetings
                 .Where(x => x.UserId == UserId)
                 .OrderByDescending(x => x.CreatedAt)
                 .ToListAsync();
        }

        public async Task<Meeting?> GetByIdAsync(Guid id, Guid UserId)
        {
            return await _db.Meetings
                 .Include(m => m.ChatMessages)
                 .FirstOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        }

        public async Task<Meeting?> GetByIdInternalAsync(Guid id)
        {
            return await _db.Meetings.FirstOrDefaultAsync(x=>x.Id == id);
        }
        

        public async Task UpdateAsync(Meeting meeting)
        {
            meeting.UpdatedAt = DateTime.UtcNow;
            _db.Meetings.Update(meeting);
            await _db.SaveChangesAsync();

        }
    }
}
