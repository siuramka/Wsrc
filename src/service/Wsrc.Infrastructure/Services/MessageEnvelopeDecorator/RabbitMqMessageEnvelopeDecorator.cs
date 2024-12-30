using RabbitMQ.Client.Events;

using Wsrc.Domain.Models;
using Wsrc.Infrastructure.Constants;

namespace Wsrc.Infrastructure.Services.MessageEnvelopeDecorator;

public class RabbitMqMessageEnvelopeDecorator(
    BasicDeliverEventArgs mqEventArgs,
    MessageEnvelope messageEnvelope
) : MessageEnvelopeDecorator(messageEnvelope)
{
    public override MessageEnvelope Decorate()
    {
        MessageEnvelope.Headers = new Dictionary<string, string>
        {
            { RabbitMqHeaders.DeliveryTag, mqEventArgs.DeliveryTag.ToString() }
        };

        return MessageEnvelope;
    }
}