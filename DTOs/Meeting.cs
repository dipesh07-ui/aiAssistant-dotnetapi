namespace aiAssistant.api.DTOs
{
   public record CreateYoutubeRequest(string Url);
    public record SubmitResponse(Guid MeetingId, string? TempFilePath = null);  
    public record MeetingListItemDto(Guid Id, string Title, string Status,
                              string SourceType, DateTime CreatedAt);
    public record MeetingDetailDto(Guid Id, string Title, string Source,
                            string SourceType, string Status,
                            string Summary, string Transcript,
                            List<string> ActionItems,
                            List<string> Decisions,
                            List<string> KeyQuestions,
                            DateTime CreatedAt,
                            string? ErrorMessage);
                            
}
