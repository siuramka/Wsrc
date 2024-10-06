using Wsrc.Core.Interfaces;
using Wsrc.Domain;

namespace Wsrc.Core.Services.Kick.EventStrategies;

public class ConnectedEvent : IKickEventStrategy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsApplicable(PusherEvent pusherEvent)
    {
        return pusherEvent.Event == PusherEvent.Connected.Event;
    }

    public async Task ExecuteAsync(string messageData)
    {
        Console.WriteLine("Connected");
    }
}
