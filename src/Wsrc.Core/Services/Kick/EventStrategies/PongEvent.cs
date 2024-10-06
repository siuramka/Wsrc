using Wsrc.Core.Interfaces;
using Wsrc.Domain;

namespace Wsrc.Core.Services.Kick.EventStrategies;

public class PongEvent : IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent)
    {
        return pusherEvent.Event == PusherEvent.Pong.Event;
    }

    public async Task ExecuteAsync(string messageData)
    {
        Console.WriteLine("PONG");
    }
}
