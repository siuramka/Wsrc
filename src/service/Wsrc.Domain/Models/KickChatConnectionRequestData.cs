using System.Text.Json.Serialization;

namespace Wsrc.Domain.Models;

public class KickChatConnectionRequestData
{
    [JsonPropertyName("auth")] public required string Auth { get; set; }
    [JsonPropertyName("channel")] public required string Channel { get; set; }
}