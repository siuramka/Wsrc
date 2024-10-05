using System.Text.Json.Serialization;

namespace Wsrc.Domain;

public class KickChatMessageIdentity
{
    [JsonPropertyName("color")] public string Color { get; set; }

    [JsonPropertyName("badges")] public KickChatMessageBadge[] Badges { get; set; }
}
