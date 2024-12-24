using System.Text.Json.Serialization;

namespace Wsrc.Domain.Models;

public class KickChatMessage
{
    [JsonPropertyName("event")] public required string Event { get; set; }

    [JsonPropertyName("data")] public required KickChatMessageChatInfo Data { get; set; }
}