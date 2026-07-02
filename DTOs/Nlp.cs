namespace aiAssistant.api.DTOs
{
    public record FullProcessResult(
        string Title,
        string Transcript,
        string Summary,
        List<string> ActionItems,
        List<string> Decisions,
        List<string> KeyQuestions
    );
}
