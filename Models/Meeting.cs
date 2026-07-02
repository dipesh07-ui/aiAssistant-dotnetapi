namespace aiAssistant.api.Models
{
    public class Meeting
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Source { get; set; }
        public string SourceType { get; set; }   // "youtube" | "file"
        public string Status { get; set; }        // "processing"|"ready"|"failed"
        public string? Transcript { get; set; }
        public string? Summary { get; set; }
        public string? ActionItems { get; set; } // JSON
        public string? Decisions { get; set; }   // JSON
        public string? KeyQuestions { get; set; }// JSON
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User User { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }
    }
}
