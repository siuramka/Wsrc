using Wsrc.Domain.Models;

namespace Wsrc.Core.Interfaces;

public interface IConsumerMessageProcessor
{
    public Task ConsumeAsync(MessageEnvelope messageEnvelope);
}