namespace aiAssistant.api.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public Guid MeetingId { get; set; }
        public string Role { get; set; }   // "user" | "assistant"
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Meeting Meeting { get; set; }
    }
}
