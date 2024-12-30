using Microsoft.Extensions.DependencyInjection;

using Wsrc.Core.Interfaces;
using Wsrc.Core.Interfaces.Mappings;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Domain.Entities;
using Wsrc.Domain.Models;

using Message = Wsrc.Domain.Entities.Message;

namespace Wsrc.Core.Services.Kick;

public class KickChatMessageBatchSavingService(
    IServiceScopeFactory serviceScopeFactory,
    IMapper mapper,
    IConsumerServiceAcknowledger acknowledger) : IKickMessageSavingService
{
    private const int MessageBatchSize = 100;
    private readonly List<ParsedKickChatMessage> _messageBatch = [];

    public async Task HandleMessageAsync(ParsedKickChatMessage parsedKickChatMessage)
    {
        _messageBatch.Add(parsedKickChatMessage);

        await CreateSenderAsync(parsedKickChatMessage.KickChatMessage);
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

        var messageEntities = _messageBatch
            .Select(m => mapper.KickChatMessageMapper.ToMessage(m.KickChatMessage))
            .ToList();

        using (var scope = serviceScopeFactory.CreateScope())
        {
            var messageRepository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<Message>>();

            await messageRepository.AddRangeAsync(messageEntities);
        }

        await AckBatchAsync();
        _messageBatch.Clear();
    }

    private async Task AckBatchAsync()
    {
        var messageEnvelopes = _messageBatch
            .Select(kcm => kcm.MessageEnvelope);

        foreach (var messageEnvelope in messageEnvelopes)
        {
            await acknowledger.AcknowledgeAsync(messageEnvelope);
        }
    }
}