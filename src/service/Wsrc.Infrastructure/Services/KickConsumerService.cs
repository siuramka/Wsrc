using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Wsrc.Core.Interfaces;
using Wsrc.Infrastructure.Constants;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services;

public class KickConsumerService(
    IRabbitMqClient rabbitMqClient,
    IKickConsumerMessageProcessor messageConsumerMessageProcessor) : IConsumerService, IAsyncDisposable
{
    private IChannel _channel;
    private IConnection _connection;

    public async Task ConnectAsync()
    {
        _connection = await rabbitMqClient.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(Queues.Wsrc, true, false, false);
        await _channel.ExchangeDeclareAsync(Exchanges.Wsrc, ExchangeType.Fanout, true, false);
        await _channel.QueueBindAsync(Queues.Wsrc, Exchanges.Wsrc, string.Empty);
    }

    public async Task ReadMessages()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnConsumerOnReceived;

        await _channel.BasicConsumeAsync(Queues.Wsrc, false, consumer);
    }

    private async Task OnConsumerOnReceived(object ch, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var messageString = Encoding.UTF8.GetString(body);

        await messageConsumerMessageProcessor.ConsumeAsync(messageString);

        await _channel.BasicAckAsync(ea.DeliveryTag, false);
    }

    public async ValueTask DisposeAsync()
    {
        await CastAndDispose(_channel);
        await CastAndDispose(_connection);

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
                await resourceAsyncDisposable.DisposeAsync();
            else
                resource.Dispose();
        }
    }
}