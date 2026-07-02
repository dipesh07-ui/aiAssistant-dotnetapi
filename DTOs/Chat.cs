namespace aiAssistant.api.DTOs
{
   public record ChatRequest(string Question);
   public record ChatResponse(string Answer);
    public record ChatMessageDto(
        Guid Id, 
        string Role,
        string Content, 
        DateTime CreatedAt
        );
}
