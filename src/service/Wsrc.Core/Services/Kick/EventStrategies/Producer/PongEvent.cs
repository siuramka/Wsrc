using Wsrc.Core.Interfaces;
using Wsrc.Domain.Models;

namespace Wsrc.Core.Services.Kick.EventStrategies.Producer;

public class PongEvent : IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent)
    {
        return pusherEvent.Event == PusherEvent.Pong.Event;
    }

    public Task ExecuteAsync(MessageEnvelope messageEnvelope)
    {
        Console.WriteLine("PONG");
        return Task.CompletedTask;
    }
}