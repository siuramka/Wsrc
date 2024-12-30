using System.Text.Json;

using Wsrc.Core.Interfaces;
using Wsrc.Domain.Models;

namespace Wsrc.Core.Services.Kick.EventStrategies.Consumer;

public class ChatMessageEvent(IKickMessageSavingService kickMessageSavingService) : IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent)
    {
        return pusherEvent.Event == PusherEvent.ChatMessage.Event;
    }

    public async Task ExecuteAsync(MessageEnvelope messageEnvelope)
    {
        var kickChatMessageBuffer = JsonSerializer
            .Deserialize<KickChatMessageBuffer>(messageEnvelope.Payload.ToString()!);

        var kickChatMessageChatInfo = JsonSerializer
            .Deserialize<KickChatMessageChatInfo>(kickChatMessageBuffer!.Data);

        var kickChatMessage = new KickChatMessage
        {
            Event = kickChatMessageBuffer.Event,
            Data = kickChatMessageChatInfo!,
        };

        var kickChatMessageWithMessage = new ParsedKickChatMessage
        {
            KickChatMessage = kickChatMessage,
            MessageEnvelope = messageEnvelope,
        };

        await kickMessageSavingService.HandleMessageAsync(kickChatMessageWithMessage);
    }
}