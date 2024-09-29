using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services;

public class KickConsumerService(IRabbitMqService rabbitMqService) : IConsumerService, IAsyncDisposable
{
    private const string _queueName = "Wsrc";

    private IChannel _channel;
    private IConnection _connection;

    public async Task ConnectAsync()
    {
        _connection = await rabbitMqService.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false);
        await _channel.ExchangeDeclareAsync("UserExchange", ExchangeType.Fanout, durable: true, autoDelete: false);
        await _channel.QueueBindAsync(_queueName, exchange: "UserExchange", routingKey: string.Empty);
    }

    public async Task ReadMessages()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnConsumerOnReceived;
        await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer);
        await Task.CompletedTask;
    }

    private async Task OnConsumerOnReceived(object ch, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var text = System.Text.Encoding.UTF8.GetString(body);
        Console.WriteLine(text);
        await Task.CompletedTask;
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