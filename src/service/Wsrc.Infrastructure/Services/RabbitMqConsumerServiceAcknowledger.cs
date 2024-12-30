using Wsrc.Core.Interfaces;
using Wsrc.Domain.Models;

namespace Wsrc.Infrastructure.Services;

public class ConsumerServiceAcknowledger(IConsumerService consumerService) : IConsumerServiceAcknowledger
{
    public async Task AcknowledgeAsync(MessageEnvelope messageEnvelope)
    {
        await consumerService.AcknowledgeAsync(messageEnvelope);
    }
}