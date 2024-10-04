using Wsrc.Domain;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services.Kick.EventStrategies;

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
