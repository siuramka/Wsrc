using System.Text.Json.Serialization;

namespace Wsrc.Domain.Models;

public class KickChatConnectionRequest(int chatroomId, PusherEvent pusherEvent)
{
    [JsonPropertyName("event")] public string Event { get; } = pusherEvent.Event;

    [JsonPropertyName("data")]
    public KickChatConnectionRequestData Data { get; } = new()
    {
        Auth = string.Empty,
        Channel = $"chatrooms.{chatroomId}.v2"
    };
}