using Wsrc.Domain.Models;

namespace Wsrc.Core.Interfaces;

public interface IProducerService
{
    Task SendMessageAsync(MessageEnvelope messageEnvelope);
}