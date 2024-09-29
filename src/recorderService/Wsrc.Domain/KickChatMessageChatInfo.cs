using System.Text.Json.Serialization;

namespace Wsrc.Domain;

public class KickChatMessageChatInfo
{
    [JsonPropertyName("id")] public Guid Id { get; set; }

    [JsonPropertyName("chatroom_id")] public long ChatroomId { get; set; }

    [JsonPropertyName("content")] public string Content { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }

    [JsonPropertyName("sender")] public KickChatMessageSender KickChatMessageSender { get; set; }
}
