using Wsrc.Core.Interfaces;
using Wsrc.Domain.Models;

namespace Wsrc.Core.Services.Kick.EventStrategies.Producer;

public class ChatMessageEvent(IProducerService producerService) : IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent)
    {
        return pusherEvent.Event == PusherEvent.ChatMessage.Event;
    }

    public async Task ExecuteAsync(string data)
    {
        await producerService.SendMessage(data);
    }
}