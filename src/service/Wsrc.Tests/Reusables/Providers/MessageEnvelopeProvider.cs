using Wsrc.Domain.Models;

namespace Wsrc.Tests.Reusables.Providers;

public class MessageEnvelopeProvider : ProviderBase<MessageEnvelope>
{
    protected override MessageEnvelope Entity { get; } = new();
}