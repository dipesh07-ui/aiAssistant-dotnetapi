namespace aiAssistant.api.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Name { get; set; }
        public string? GoogleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Meeting> Meetings { get; set; }
    }
}
