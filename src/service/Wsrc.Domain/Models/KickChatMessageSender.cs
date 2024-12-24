using System.Text.Json.Serialization;

namespace Wsrc.Domain.Models;

public class KickChatMessageSender
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("username")] public required string Username { get; set; }

    [JsonPropertyName("slug")] public required string Slug { get; set; }

    [JsonPropertyName("identity")] public required KickChatMessageIdentity KickChatMessageIdentity { get; set; }
}