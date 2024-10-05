using System.Text.Json.Serialization;

namespace Wsrc.Domain;

public class KickChatMessage
{
    [JsonPropertyName("event")] public string Event { get; set; }

    [JsonPropertyName("data")] public KickChatMessageChatInfo Data { get; set; }

    [JsonPropertyName("channel")] public string Channel { get; set; }
}
