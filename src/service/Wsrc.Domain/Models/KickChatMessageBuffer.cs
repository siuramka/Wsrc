using System.Text.Json.Serialization;

namespace Wsrc.Domain.Models;

public class KickChatMessageBuffer
{
    [JsonPropertyName("event")] public string Event { get; set; }

    [JsonPropertyName("data")] public string Data { get; set; }

    [JsonPropertyName("channel")] public string Channel { get; set; }
}