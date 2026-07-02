using aiAssistant.api.Common;
using aiAssistant.api.DTOs;

namespace aiAssistant.api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
        Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<Result<AuthResponse>> GoogleAuthAsync(GoogleAuthRequest request);
        Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken);
    }
}
