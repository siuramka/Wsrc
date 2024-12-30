using Wsrc.Domain.Models;

namespace Wsrc.Infrastructure.Services.MessageEnvelopeDecorator;

public abstract class MessageEnvelopeDecorator(MessageEnvelope messageEnvelope)
{
    protected MessageEnvelope MessageEnvelope { get; set; } = messageEnvelope;

    public abstract MessageEnvelope Decorate();
}