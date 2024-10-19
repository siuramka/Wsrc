using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Wsrc.Core.Interfaces;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Domain;
using Wsrc.Domain.Entities;
using Wsrc.Domain.Models;

namespace Wsrc.Core.Services.Kick;

public class KickConsumerMessageProcessor(
    IKickMessageSavingService kickMessageSavingService
)
    : IKickConsumerMessageProcessor
{
    public async Task ConsumeAsync(string data)
    {
        var chatMessage = JsonSerializer
                              .Deserialize<KickEvent>(data)
                          ?? throw new Exception("Failed to deserialize message");

        var pusherEvent = PusherEvent.Parse(chatMessage.Event);

        if (pusherEvent == PusherEvent.ChatMessage)
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
}