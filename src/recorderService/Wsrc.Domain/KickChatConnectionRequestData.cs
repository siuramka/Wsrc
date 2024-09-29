using System.Text.Json.Serialization;

namespace Wsrc.Domain;

public class KickChatConnectionRequestData
{
    [JsonPropertyName("auth")] public string Auth { get; set; }
    [JsonPropertyName("channel")] public string Channel { get; set; }
}