using Wsrc.Core.Interfaces;
using Wsrc.Domain;

namespace Wsrc.Core.Services.Kick.EventStrategies;

public class PongEvent : IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent)
    {
        return pusherEvent.Event == PusherEvent.Pong.Event;
    }

    public Task ExecuteAsync(string data)
    {
        Console.WriteLine("PONG");
        return Task.CompletedTask;
    }
}
