using System.Text.Json.Serialization;

namespace Wsrc.Domain.Models;

public class KickChatMessageBadge
{
    [JsonPropertyName("type")] public required string Type { get; set; }

    [JsonPropertyName("text")] public required string Text { get; set; }

    [JsonPropertyName("count")] public long Count { get; set; }
}