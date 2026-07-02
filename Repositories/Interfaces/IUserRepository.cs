using aiAssistant.api.Models;

namespace aiAssistant.api.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByGoogleIdAsync(string googleId);
        Task<User> CreateUserAsync(User user);
        Task<bool> UserExistsAsync(string email);
    }
}
