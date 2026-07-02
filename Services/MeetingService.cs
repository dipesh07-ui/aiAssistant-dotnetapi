using aiAssistant.api.BackgroundServices;
using aiAssistant.api.Common;
using aiAssistant.api.DTOs;
using aiAssistant.api.Extensions;
using aiAssistant.api.Models;
using aiAssistant.api.Repositories.Interfaces;
using aiAssistant.api.Services.Interfaces;

namespace aiAssistant.api.Services
{
    public class MeetingService(IMeetingRepository _repo) : IMeetingService
    {
        public async Task<Result> DeleteAsync(Guid id, Guid userId)
        {
            var meeting = await _repo.GetByIdAsync(id, userId);
            if(meeting ==null) return Result.Failure("Meeting not found"); 
            await _repo.DeleteAsync(id,userId);
            return Result.Success();

        }

        public async Task<Result<PagedResult<MeetingListItemDto>>> GetAllAsync(Guid userId, int page, int pageSize)
        {
            var meetings = await _repo.GetAllAsync(userId);
            var list= meetings.ToList();
            var paged = list
           .Skip((page - 1) * pageSize)
           .Take(pageSize)
           .Select(m => m.ToListItemDto())
           .ToList();

            var result = new PagedResult<MeetingListItemDto>
            {
                Items = paged,
                TotalCount = list.Count,
                Page = page,
                PageSize = pageSize
            };
            return Result<PagedResult<MeetingListItemDto>>.Success(result);
        }

        public async Task<Result<MeetingDetailDto>> GetByIdAsync(Guid id, Guid userId)
        {
            var meeting = await _repo.GetByIdAsync(id, userId);
            if(meeting is null) return Result<MeetingDetailDto>.Failure("Meeting not found");

            return Result<MeetingDetailDto>.Success(meeting.ToDetailDto());
        }

        public async Task<Result<SubmitResponse>> SubmitFileAsync(Guid userId, IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName);
            var tmpPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{ext}");
            await using var stream = File.Create(tmpPath);
            await file.CopyToAsync(stream);

            var meeting = new Meeting
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Source = tmpPath,
                SourceType = "file",
                Status = "processing",
                Title = Path.GetFileNameWithoutExtension(file.FileName),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repo.CreateAsync(meeting);
            return Result<SubmitResponse>.Success(
                new SubmitResponse(meeting.Id, tmpPath));
        }

        public async Task<Result<SubmitResponse>> SubmitYoutubeAsync(Guid userId, CreateYoutubeRequest request)
        {
            var meeting = new Meeting
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Source = request.Url,
                SourceType = "youtube",
                Status= "processing",
                Title = "processing",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _repo.CreateAsync(meeting);
            return Result<SubmitResponse>.Success(new SubmitResponse(meeting.Id));
        }
    }
}
