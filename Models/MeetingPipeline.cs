namespace aiAssistant.api.Models
{
    public class PipelineJob
    {
        public Guid MeetingId { get; set; }
        public string Source { get; set; } = "";
        public string SourceType { get; set; } = "";
        public Guid UserId { get; set; }
    }

    public class ProgressEvent
    {
        public string Step { get; set; } = "";
        public string Message { get; set; } = "";
        public int Percent { get; set; }
        public int? Chunk { get; set; }
        public int? TotalChunks { get; set; }
    }

}
