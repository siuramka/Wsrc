using System.Text.Json.Serialization;

using Wsrc.Domain.Models;

namespace Wsrc.Domain;

public class KickChatMessage
{
    [JsonPropertyName("event")] public string Event { get; set; }

    [JsonPropertyName("data")] public KickChatMessageChatInfo Data { get; set; }
}