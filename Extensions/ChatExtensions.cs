using aiAssistant.api.DTOs;
using aiAssistant.api.Models;

namespace aiAssistant.api.Extensions
{
    public static class ChatExtensions
    {
        public static ChatMessageDto ToDto(this ChatMessage m) =>
            new(m.Id, m.Role, m.Content, m.CreatedAt);
       
    }
}
