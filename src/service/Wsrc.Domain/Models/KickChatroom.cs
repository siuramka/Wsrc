using System.Text.Json.Serialization;

namespace Wsrc.Domain;

public class KickChatroom
{
    [JsonPropertyName("id")] public int Id { get; set; }
}