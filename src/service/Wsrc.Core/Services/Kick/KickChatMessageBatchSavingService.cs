using Microsoft.Extensions.DependencyInjection;
using Wsrc.Core.Interfaces;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Domain;
using Wsrc.Domain.Entities;

namespace Wsrc.Core.Services.Kick;

public class KickChatMessageBatchSavingService(IServiceScopeFactory serviceScopeFactory) : IKickMessageSavingService
{
    private const int MessageBatchSize = 100;
    private readonly List<Message> _messageBatch = [];

    public async Task HandleMessageAsync(KickChatMessage kickChatMessage)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var senderRepository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<Sender>>();

        var message = new Message
        {
            ChatroomId = kickChatMessage.Data.ChatroomId,
            Content = kickChatMessage.Data.Content,
            Timestamp = kickChatMessage.Data.CreatedAt.ToUniversalTime(),
            SenderId = kickChatMessage.Data.KickChatMessageSender.Id,
        };

        _messageBatch.Add(message);

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

            await senderRepository.AddAsync(newSender);
        }

        await FlushBatchesAsync();
    }

    private async Task FlushBatchesAsync()
    {
        if (_messageBatch.Count < MessageBatchSize)
        {
            return;
        }

        using var scope = serviceScopeFactory.CreateScope();
        var messageRepository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<Message>>();

        await messageRepository.AddRangeAsync(_messageBatch);

        _messageBatch.Clear();
    }
}