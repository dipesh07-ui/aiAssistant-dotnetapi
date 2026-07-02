using aiAssistant.api.Common;
using aiAssistant.api.DTOs;

namespace aiAssistant.api.Services.Interfaces
{
    public interface INlpService
    {
        Task<Result<FullProcessResult>> FullProcessAsync(string source, string sourceType);
        Task<Result<string>> AskAsync(string transcript, string question);
    }
}
