using Wsrc.Domain;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services.Kick.EventStrategies;

public class ConnectedEvent : IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent)
    {
        return pusherEvent.Event == PusherEvent.Connected.Event;
    }

    public async Task ExecuteAsync(string messageData)
    {
        Console.WriteLine("Connected");
    }
}
