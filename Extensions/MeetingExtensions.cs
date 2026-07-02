using aiAssistant.api.DTOs;
using aiAssistant.api.Models;
using System.Text.Json;

namespace aiAssistant.api.Extensions
{
    public static class MeetingExtensions
    {
        public static MeetingListItemDto ToListItemDto(this Meeting m) =>
        new(m.Id, m.Title, m.Status, m.SourceType, m.CreatedAt);

        public static MeetingDetailDto ToDetailDto(this Meeting m) =>
            new(m.Id, m.Title, m.Source, m.SourceType, m.Status,
                m.Summary ?? "", m.Transcript ?? "",
                JsonSerializer.Deserialize<List<string>>(m.ActionItems ?? "[]")!,
                JsonSerializer.Deserialize<List<string>>(m.Decisions ?? "[]")!,
                JsonSerializer.Deserialize<List<string>>(m.KeyQuestions ?? "[]")!,
                m.CreatedAt,
                m.ErrorMessage);
    }
}
