using System.Text.Json.Serialization;

namespace Wsrc.Domain;

public class KickChatMessageSender
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("username")] public string Username { get; set; }

    [JsonPropertyName("slug")] public string Slug { get; set; }

    [JsonPropertyName("identity")] public KickChatMessageIdentity KickChatMessageIdentity { get; set; }
}
