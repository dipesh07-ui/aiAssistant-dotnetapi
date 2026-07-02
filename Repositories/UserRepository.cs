using aiAssistant.api.Data;
using aiAssistant.api.Models;
using aiAssistant.api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace aiAssistant.api.Repositories
{
    public class UserRepository(AppDbContext _db) : IUserRepository
    {
        public async Task<User> CreateUserAsync(User user)
        {
            await _db.AddAsync(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
           
        }

        public async Task<User?> GetByGoogleIdAsync(string googleId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> UserExistsAsync(string email)
        {
           return  await _db.Users.AnyAsync(u => u.Email == email);
           
        }
    }
}
