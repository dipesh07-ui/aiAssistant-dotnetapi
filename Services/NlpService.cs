using aiAssistant.api.Common;
using aiAssistant.api.DTOs;
using aiAssistant.api.Services.Interfaces;
using System.Text.Json;

namespace aiAssistant.api.Services
{
    // Services/NlpService.cs
    public class NlpService(HttpClient _http, IConfiguration _config) : INlpService
    {
        public async Task<Result<FullProcessResult>> FullProcessAsync(
            string source, string sourceType)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(
                    $"{_config["NlpApi:BaseUrl"]}/full-process",
                    new { source, source_type = sourceType }
                );
                response.EnsureSuccessStatusCode();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                };

                var result = await response.Content
                    .ReadFromJsonAsync<FullProcessResult>();
                return Result<FullProcessResult>.Success(result!);
            }
            catch (Exception ex)
            {
                return Result<FullProcessResult>.Failure(
                    $"Processing failed: {ex.Message}");
            }
        }

        public async Task<Result<string>> AskAsync(string transcript, string question)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(
                    $"{_config["NlpApi:BaseUrl"]}/chat",
                    new { transcript, question }
                );
                response.EnsureSuccessStatusCode();
                var result = await response.Content
                    .ReadFromJsonAsync<ChatResponse>();
                return Result<string>.Success(result!.Answer);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Chat failed: {ex.Message}");
            }
        }
    }

    record ChatResponse(string Answer);
}

