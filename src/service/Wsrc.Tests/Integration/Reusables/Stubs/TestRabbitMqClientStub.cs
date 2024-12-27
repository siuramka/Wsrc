using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Constants;

namespace Wsrc.Tests.Integration.Reusables.Stubs;

public class TestRabbitMqClientStub : IAsyncDisposable
{
    private IChannel _channel = null!;
    private IConnection _connection = null!;

    public async Task ConnectAsync(RabbitMqConfiguration config)
    {
        await ConnectAsync(
            new ConnectionFactory { UserName = config.Username, Port = config.Port, Password = config.Password, });
    }

    public async Task PublishMessageAsync(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        var basicProperties = new BasicProperties { Persistent = true };

        await _channel.BasicPublishAsync(
            exchange: Exchanges.Wsrc,
            routingKey: Queues.Wsrc,
            mandatory: true,
            basicProperties,
            body,
            CancellationToken.None
        );
    }

    public async Task ConsumeMessagesAsync(AsyncEventHandler<BasicDeliverEventArgs> onConsumerReceivedAsync)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += onConsumerReceivedAsync;

        await _channel.BasicConsumeAsync(Queues.Wsrc, false, consumer);
    }

    private async Task ConnectAsync(ConnectionFactory factory)
    {
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await SetupQueueAsync();
    }

    private async Task SetupQueueAsync()
    {
        await _channel.QueueDeclareAsync(Queues.Wsrc, durable: true, exclusive: false, autoDelete: false);
        await _channel.ExchangeDeclareAsync(Exchanges.Wsrc, ExchangeType.Fanout, durable: true, autoDelete: false);
        await _channel.QueueBindAsync(Queues.Wsrc, exchange: Exchanges.Wsrc, string.Empty);
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }
}