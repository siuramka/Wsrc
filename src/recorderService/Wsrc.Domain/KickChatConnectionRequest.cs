using System.Text.Json.Serialization;

namespace Wsrc.Domain;

public class KickChatConnectionRequest(string chatroomId, PusherEvent pusherEvent)
{
    [JsonPropertyName("event")] public string Event { get; } = pusherEvent.Name;
    [JsonPropertyName("data")] public KickChatConnectionRequestData Data { get; } = new()
    {
        Auth = string.Empty,
        Channel = $"chatrooms.{chatroomId}.v2"
    };
}