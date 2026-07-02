using aiAssistant.api.Common;
using aiAssistant.api.DTOs;
using aiAssistant.api.Extensions;
using aiAssistant.api.Models;
using aiAssistant.api.Repositories.Interfaces;
using aiAssistant.api.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace aiAssistant.api.Services
{
    public class AuthService(
     IUserRepository _userRepo,
     IConfiguration _config,
     IPasswordHasher<User> _hasher) : IAuthService
    {
        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            var result = _hasher.VerifyHashedPassword(
        user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
                return Result<AuthResponse>.Failure("Invalid credentials");

            var token = GenerateJwt(user);
            var refresh = GenerateRefreshToken();
            return Result<AuthResponse>.Success(new AuthResponse(token, refresh, user.ToDto()));
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepo.UserExistsAsync(request.Email))
                return Result<AuthResponse>.Failure("Email already registered");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Name,
                PasswordHash= "", 
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);

            await _userRepo.CreateUserAsync(user);
            var token = GenerateJwt(user);
            var refresh = GenerateRefreshToken();
            return Result<AuthResponse>.Success(new AuthResponse(token, refresh, user.ToDto()));
        }

        public async Task<Result<AuthResponse>> GoogleAuthAsync(GoogleAuthRequest request)
        {
            // validate Google token
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.GoogleToken);
            if (payload == null)
                return Result<AuthResponse>.Failure("Invalid Google token");

            // find or create user
            var user = await _userRepo.GetByGoogleIdAsync(payload.Subject)
                    ?? await _userRepo.GetByEmailAsync(payload.Email);

            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = payload.Email,
                    Name = payload.Name,
                    GoogleId = payload.Subject,
                    CreatedAt = DateTime.UtcNow
                };
                await _userRepo.CreateUserAsync(user);
            }

            var token = GenerateJwt(user);
            var refresh = GenerateRefreshToken();
            return Result<AuthResponse>.Success(new AuthResponse(token, refresh, user.ToDto()));
        }

        public Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken)
        {
            // simplified — in production store refresh tokens in DB
            throw new NotImplementedException();
        }

        private string GenerateJwt(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken() =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
