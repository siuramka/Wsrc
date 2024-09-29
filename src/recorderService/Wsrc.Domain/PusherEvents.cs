namespace Wsrc.Domain;

public class PusherEvent
{
    public static readonly PusherEvent Subscribe = new() { Name = "pusher:subscribe" };

    public static readonly PusherEvent ChatMessage = new() { Name = @"App\Events\ChatMessageEvent" };

    public static readonly PusherEvent Pong = new() { Name = "pusher:pong" };

    public static readonly PusherEvent Connected = new() { Name = "pusher:connection_established" };

    public static readonly PusherEvent Subscribed = new() { Name = "pusher_internal:subscription_succeeded" };
    public string Name { get; private init; }
}
