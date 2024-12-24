using System.Text.Json.Serialization;

namespace Wsrc.Domain;

public class KickChatMessageBadge
{
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("text")] public string Text { get; set; }

    [JsonPropertyName("count")] public long Count { get; set; }
}