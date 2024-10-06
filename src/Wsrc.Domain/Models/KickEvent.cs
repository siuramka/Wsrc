using System.Text.Json.Serialization;

namespace Wsrc.Domain;

public class KickEvent
{
    [JsonPropertyName("event")]
    public required string Event { get; set; }
}
