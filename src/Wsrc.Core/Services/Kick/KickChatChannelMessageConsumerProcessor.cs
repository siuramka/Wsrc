using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Wsrc.Core.Interfaces;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Domain;
using Wsrc.Domain.Entities;
using Wsrc.Domain.Models;

namespace Wsrc.Core.Services.Kick;

public class KickChatChannelMessageConsumerProcessor(IServiceScopeFactory serviceScopeFactory)
    : IKickChatChannelMessageConsumerProcessor
{
    private const int MessageBatchSize = 10;
    private List<Message> _messageBatch = [];
    private List<Sender> _sendersBatch = [];

    public async Task ConsumeAsync(string data)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var messageRepository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<Message>>();
        var senderRepository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<Sender>>();
        var chatroomRepository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<Chatroom>>();

        var chatMessage = JsonSerializer.Deserialize<KickEvent>(data)
                          ?? throw new Exception("Failed to deserialize message"); //todo check these?

        var pusherEvent = PusherEvent.Parse(chatMessage.Event);

        if (pusherEvent == PusherEvent.ChatMessage)
        {
            var kickChatMessageBuffer = JsonSerializer.Deserialize<KickChatMessageBuffer>(data);

            var kickChatMessageChatInfo =
                JsonSerializer.Deserialize<KickChatMessageChatInfo>(kickChatMessageBuffer.Data);

            var kickChatMessage = new KickChatMessage
            {
                Event = kickChatMessageBuffer.Event,
                Data = kickChatMessageChatInfo,
                Channel = kickChatMessageBuffer.Channel,
            };

            var message = new Message
            {
                ChatroomId = kickChatMessage.Data.ChatroomId,
                Content = kickChatMessage.Data.Content,
                Timestamp = kickChatMessage.Data.CreatedAt.ToUniversalTime(),
                SenderId = kickChatMessage.Data.KickChatMessageSender.Id,
            };

            _messageBatch.Add(message);

            var chatroom = await chatroomRepository
                .FirstOrDefaultAsync(x => x.Id == kickChatMessage.Data.ChatroomId);

            // TODO: move to producer
            if (chatroom is null)
            {
                var newChatroom = new Chatroom
                {
                    Id = kickChatMessage.Data.ChatroomId,
                    Username = kickChatMessage.Channel.ToLower(),
                };

                await chatroomRepository.AddAsync(newChatroom);
            }

            var sender = await senderRepository
                .FirstOrDefaultAsync(x => x.Id == kickChatMessage.Data.KickChatMessageSender.Id);

            if (sender is null)
            {
                var newSender = new Sender
                {
                    Id = kickChatMessage.Data.KickChatMessageSender.Id,
                    Username = kickChatMessage.Data.KickChatMessageSender.Username.ToLower(),
                    Slug = kickChatMessage.Data.KickChatMessageSender.Slug.ToLower(),
                };

                if (!_sendersBatch.Contains(newSender))
                {
                    _sendersBatch.Add(newSender);
                }
            }

            if (_messageBatch.Count >= MessageBatchSize)
            {
                await senderRepository.AddRangeAsync(_sendersBatch);
                _sendersBatch.Clear();

                await messageRepository.AddRangeAsync(_messageBatch);
                _messageBatch.Clear();
            }
        }
    }
}