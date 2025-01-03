using System.Text;

using RabbitMQ.Client;

using Wsrc.Core.Interfaces;
using Wsrc.Domain.Models;
using Wsrc.Infrastructure.Constants;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Messaging;

public class RabbitMqProducer(IRabbitMqClient rabbitMqClient) : IProducerService
{
    public async Task SendMessageAsync(MessageEnvelope messageEnvelope)
    {
        await using var connection = await rabbitMqClient.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        var body = Encoding.UTF8.GetBytes(messageEnvelope.Payload.ToString()!);

        var basicProperties = new BasicProperties { Persistent = true };

        await channel.BasicPublishAsync(
            exchange: Exchanges.Wsrc,
            routingKey: Queues.Wsrc,
            mandatory: true,
            basicProperties,
            body,
            CancellationToken.None
        );
    }
}