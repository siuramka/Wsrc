using System.Text.Json;
using Wsrc.Core.Interfaces;
using Wsrc.Domain;

namespace Wsrc.Core.Services.Kick.EventStrategies;

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