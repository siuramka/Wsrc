using Wsrc.Domain;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services.Kick.EventStrategies;

public class ChatMessageEvent(IProducerService producerService) : IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent)
    {
        return pusherEvent.Event == PusherEvent.ChatMessage.Event;
    }

    public async Task ExecuteAsync(string messageData)
    {
        await producerService.SendMessage(messageData);
    }
}
