using System.Text.Json.Serialization;

namespace Wsrc.Domain.Models;

public class KickChatMessageBuffer
{
    [JsonPropertyName("event")] public required string Event { get; set; }

    [JsonPropertyName("data")] public required string Data { get; set; }

    [JsonPropertyName("channel")] public required string Channel { get; set; }
}