using Wsrc.Core.Interfaces;
using Wsrc.Domain.Models;

namespace Wsrc.Core.Services.Kick.EventStrategies.Producer;

public class SubscribedEvent : IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent)
    {
        return pusherEvent.Event == PusherEvent.Subscribed.Event;
    }

    public Task ExecuteAsync(string data)
    {
        Console.WriteLine("Subscribed");
        return Task.CompletedTask;
    }
}