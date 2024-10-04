using Wsrc.Domain;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services.Kick.EventStrategies;

public class SubscribedEvent(IKickPusherClientManager clientManager) : IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent)
    {
        return pusherEvent.Event == PusherEvent.Subscribed.Event;
    }

    public async Task ExecuteAsync(string messageData)
    {
        Console.WriteLine("Subscribed");
    }
}
