using aiAssistant.api.Models;

namespace aiAssistant.api.Repositories.Interfaces
{
    public interface IMeetingRepository
    {
        Task<Meeting?> GetByIdAsync(Guid id,Guid UserId);
        Task<Meeting?> GetByIdInternalAsync(Guid id);
        Task<IEnumerable<Meeting>> GetAllAsync(Guid UserId);
        Task<Meeting> CreateAsync(Meeting meeting);
        Task UpdateAsync(Meeting meeting);
        Task DeleteAsync(Guid id, Guid userId);
    }
}
