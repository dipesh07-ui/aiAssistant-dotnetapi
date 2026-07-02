using aiAssistant.api.Common;
using aiAssistant.api.DTOs;

namespace aiAssistant.api.Services.Interfaces
{
    public interface IMeetingService
    {
        Task<Result<PagedResult<MeetingListItemDto>>> GetAllAsync(Guid userId,int page, int pageSize);
        Task<Result<MeetingDetailDto>> GetByIdAsync(Guid id, Guid userId);
        Task<Result<SubmitResponse>> SubmitYoutubeAsync(Guid userId, CreateYoutubeRequest request);
        Task<Result<SubmitResponse>> SubmitFileAsync(Guid userId, IFormFile file);
        Task<Result> DeleteAsync(Guid id, Guid userId);
    }
}
