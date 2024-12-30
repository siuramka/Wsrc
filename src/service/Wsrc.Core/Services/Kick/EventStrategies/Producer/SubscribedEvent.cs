using Wsrc.Core.Interfaces;
using Wsrc.Domain.Models;

namespace Wsrc.Core.Services.Kick.EventStrategies.Producer;

public class SubscribedEvent : IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent)
    {
        return pusherEvent.Event == PusherEvent.Subscribed.Event;
    }

    public Task ExecuteAsync(MessageEnvelope messageEnvelope)
    {
        Console.WriteLine("Subscribed");
        return Task.CompletedTask;
    }
}