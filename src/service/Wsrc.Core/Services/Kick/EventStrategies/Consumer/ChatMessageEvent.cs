using System.Text.Json;
using Wsrc.Core.Interfaces;
using Wsrc.Domain;
using Wsrc.Domain.Models;

namespace Wsrc.Core.Services.Kick.EventStrategies.Consumer;

public class ChatMessageEvent(IKickMessageSavingService kickMessageSavingService) : IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent)
    {
        return pusherEvent.Event == PusherEvent.ChatMessage.Event;
    }

    public async Task ExecuteAsync(string data)
    {
        var kickChatMessageBuffer = JsonSerializer
                                        .Deserialize<KickChatMessageBuffer>(data)
                                    ?? throw new Exception("Failed to deserialize message");

        var kickChatMessageChatInfo = JsonSerializer
                                          .Deserialize<KickChatMessageChatInfo>(kickChatMessageBuffer.Data)
                                      ?? throw new Exception("Failed to deserialize message");

        var kickChatMessage = new KickChatMessage
        {
            Event = kickChatMessageBuffer.Event,
            Data = kickChatMessageChatInfo,
        };

        await kickMessageSavingService.HandleMessageAsync(kickChatMessage);
    }
}
