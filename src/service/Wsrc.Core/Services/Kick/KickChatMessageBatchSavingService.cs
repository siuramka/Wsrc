using Microsoft.Extensions.DependencyInjection;
using Wsrc.Core.Interfaces;
using Wsrc.Core.Interfaces.Mappings;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Domain;
using Wsrc.Domain.Entities;

namespace Wsrc.Core.Services.Kick;

public class KickChatMessageBatchSavingService(
    IServiceScopeFactory serviceScopeFactory,
    IMapper mapper) : IKickMessageSavingService
{
    private const int MessageBatchSize = 10;
    private readonly List<Message> _messageBatch = [];

    public async Task HandleMessageAsync(KickChatMessage kickChatMessage)
    {
        var message = mapper.KickChatMessageMapper.ToMessage(kickChatMessage);

        _messageBatch.Add(message);

        await CreateSenderAsync(kickChatMessage);
        await FlushBatchesAsync();
    }

    private async Task CreateSenderAsync(KickChatMessage kickChatMessage)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var senderRepository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<Sender>>();

        var sender = await senderRepository
            .FirstOrDefaultAsync(s => s.Id == kickChatMessage.Data.KickChatMessageSender.Id);

        if (sender is not null)
        {
            return;
        }

        var newSender = mapper.KickChatMessageMapper.ToSender(kickChatMessage);

        await senderRepository.AddAsync(newSender);
    }

    private async Task FlushBatchesAsync()
    {
        if (_messageBatch.Count < MessageBatchSize)
        {
            return;
        }

        var messages = new List<Message>(_messageBatch);

        using (var scope = serviceScopeFactory.CreateScope())
        {
            var messageRepository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<Message>>();

            await messageRepository.AddRangeAsync(messages);
        }

        _messageBatch.Clear();
    }
}
