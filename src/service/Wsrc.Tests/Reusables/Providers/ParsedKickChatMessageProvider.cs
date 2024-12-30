using Wsrc.Domain.Models;
using Wsrc.Infrastructure.Constants;

namespace Wsrc.Tests.Reusables.Providers;

public class ParsedKickChatMessageProvider : ProviderBase<ParsedKickChatMessage>
{
    protected override ParsedKickChatMessage Entity { get; } = new()
    {
        KickChatMessage = new KickChatMessageProvider().Create(),
        MessageEnvelope = new MessageEnvelopeProvider().Create(),
    };

    public ParsedKickChatMessage CreateWithDeliveryTag(ulong deliveryTag)
    {
        Entity.MessageEnvelope.Headers[RabbitMqHeaders.DeliveryTag] = deliveryTag.ToString();

        return Entity;
    }
}