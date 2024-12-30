using Wsrc.Domain.Models;

namespace Wsrc.Core.Interfaces;

public interface IConsumerServiceAcknowledger
{
    Task AcknowledgeAsync(MessageEnvelope messageEnvelope);
}