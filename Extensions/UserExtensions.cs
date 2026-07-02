using aiAssistant.api.DTOs;
using aiAssistant.api.Models;

namespace aiAssistant.api.Extensions
{
    public static class UserExtensions
    {
        public static UserDto ToDto(this User u)
        {
            return new(u.Id, u.Email, u.Name ?? u.Email);
        }
    }
}
