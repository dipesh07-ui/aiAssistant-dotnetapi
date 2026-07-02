using aiAssistant.api.Common;
using aiAssistant.api.DTOs;
using aiAssistant.api.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace aiAssistant.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService,
    IValidator<LoginRequest> _loginValidator,
    IValidator<RegisterRequest> _registerValidator) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var validation = await _loginValidator.ValidateAsync(request);
            if (!validation.IsValid)
                return BadRequest(ApiResponse<AuthResponse>
                    .Fail(validation.Errors.First().ErrorMessage));

            var result = await _authService.LoginAsync(request);
            if (!result.IsSuccess)
                return Unauthorized(ApiResponse<AuthResponse>.Fail(result.Error!));

            return Ok(ApiResponse<AuthResponse>.Ok(result.Value!));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var validation = await _registerValidator.ValidateAsync(request);
            if (!validation.IsValid)
                return BadRequest(ApiResponse<AuthResponse>
                    .Fail(validation.Errors.First().ErrorMessage));

            var result = await _authService.RegisterAsync(request);
            if (!result.IsSuccess)
                return BadRequest(ApiResponse<AuthResponse>.Fail(result.Error!));

            return Ok(ApiResponse<AuthResponse>.Ok(result.Value!));
        }

        [HttpPost("google")]

        public async Task<IActionResult> GoogleAuth(GoogleAuthRequest request)
        {
            var result = await _authService.GoogleAuthAsync(request);
            if (!result.IsSuccess)
                return Unauthorized(ApiResponse<AuthResponse>.Fail(result.Error!));
            return Ok(ApiResponse<AuthResponse>.Ok(result.Value!));
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            return Ok(ApiResponse<object>.Ok(new { userId, email }));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (!result.IsSuccess)
                return Unauthorized(ApiResponse<AuthResponse>.Fail(result.Error!));

            return Ok(ApiResponse<AuthResponse>.Ok(result.Value!));
        }
    }
}
