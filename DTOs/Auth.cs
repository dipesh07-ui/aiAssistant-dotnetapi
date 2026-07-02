namespace aiAssistant.api.DTOs
{
    public record LoginRequest(string Email, string Password);
    public  record RegisterRequest(string Name, string Email, string Password);
    public  record GoogleAuthRequest(string GoogleToken);
    public  record AuthResponse(string Token, string RefreshToken, UserDto User);
    public  record UserDto(Guid Id, string Email, string Name);
}
