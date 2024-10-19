using System.Text;
using RabbitMQ.Client;
using Wsrc.Core.Interfaces;
using Wsrc.Infrastructure.Constants;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Messaging;

public class RabbitMqProducer(IRabbitMqClient rabbitMqClient) : IProducerService
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public async Task SendMessage(string message)
    {
        using var connection = await rabbitMqClient.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        var body = Encoding.UTF8.GetBytes(message);

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
