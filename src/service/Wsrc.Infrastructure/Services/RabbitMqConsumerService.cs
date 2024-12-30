using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Wsrc.Core.Interfaces;
using Wsrc.Domain.Models;
using Wsrc.Infrastructure.Constants;
using Wsrc.Infrastructure.Interfaces;
using Wsrc.Infrastructure.Services.MessageEnvelopeDecorator;

namespace Wsrc.Infrastructure.Services;

public class RabbitMqConsumerService(
    IRabbitMqClient rabbitMqClient,
    IConsumerMessageProcessor messageProcessor)
    : IConsumerService, IAsyncDisposable
{
    private IChannel _channel = null!;
    private IConnection _connection = null!;

    public async Task ConnectAsync()
    {
        _connection = await rabbitMqClient.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await SetupQueueAsync();
    }

    private async Task SetupQueueAsync()
    {
        await _channel.QueueDeclareAsync(Queues.Wsrc, durable: true, exclusive: false, autoDelete: false);
        await _channel.ExchangeDeclareAsync(Exchanges.Wsrc, ExchangeType.Fanout, durable: true, autoDelete: false);
        await _channel.QueueBindAsync(Queues.Wsrc, exchange: Exchanges.Wsrc, string.Empty);
    }

    public async Task ConsumeMessagesAsync()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnConsumerOnReceivedAsync;

        await _channel.BasicConsumeAsync(Queues.Wsrc, false, consumer);
    }

    private async Task OnConsumerOnReceivedAsync(object ch, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var messageString = Encoding.UTF8.GetString(body);

        var message = new MessageEnvelope { Payload = messageString };

        var rabbitMqMessageDecorator = new RabbitMqMessageEnvelopeDecorator(ea, message);

        var decorated = rabbitMqMessageDecorator.Decorate();

        await messageProcessor.ConsumeAsync(decorated);
    }

    public async Task AcknowledgeAsync(MessageEnvelope messageEnvelope)
    {
        var deliveryTagString = messageEnvelope.Headers[RabbitMqHeaders.DeliveryTag];
        var deliveryTag = ulong.Parse(deliveryTagString);

        await _channel.BasicAckAsync(deliveryTag, false);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }
}