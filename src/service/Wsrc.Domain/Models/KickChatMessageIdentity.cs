using System.Text.Json.Serialization;

namespace Wsrc.Domain;

public class KickChatMessageIdentity
{
    [JsonPropertyName("color")] public string Color { get; set; } = "#FF0000";

    [JsonPropertyName("badges")] public IEnumerable<KickChatMessageBadge> Badges { get; set; } = [];
}
