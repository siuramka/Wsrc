using Wsrc.Domain.Models;

namespace Wsrc.Core.Interfaces;

public interface IConsumerService
{
    Task ConnectAsync();

    Task ConsumeMessagesAsync();

    Task AcknowledgeAsync(MessageEnvelope messageEnvelope);
}