namespace Wsrc.Domain.Models;

public class PusherEvent
{
    public static readonly PusherEvent Subscribe = new() { Event = "pusher:subscribe" };

    public static readonly PusherEvent ChatMessage = new() { Event = @"App\Events\ChatMessageEvent" };

    public static readonly PusherEvent Pong = new() { Event = "pusher:pong" };

    public static readonly PusherEvent Connected = new() { Event = "pusher:connection_established" };

    public static readonly PusherEvent Subscribed = new() { Event = "pusher_internal:subscription_succeeded" };

    public string Event { get; private init; }

    public static PusherEvent Parse(string eventName)
    {
        return eventName switch
        {
            "pusher:subscribe" => Subscribe,
            @"App\Events\ChatMessageEvent" => ChatMessage,
            "pusher:pong" => Pong,
            "pusher:connection_established" => Connected,
            "pusher_internal:subscription_succeeded" => Subscribed,
            _ => throw new ArgumentException($"Unknown event: {eventName}")
        };
    }
}