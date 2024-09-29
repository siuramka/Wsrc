using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Wsrc.Domain;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services.Kick;

public class KickProducerFacade(
    IOptions<KickConfiguration> kick,
    IKickPusherClientFactory pusherClientFactory,
    IRabbitMqClient rabbitMqClient,
    IProducerService producerService)
{
    public async Task HandleMessages()
    {
        var kickPusherClients = pusherClientFactory.CreateClients(kick.Value.Channels);

        foreach (var kickPusherClient in kickPusherClients)
        {
            var task = Task.Run(() => Task.FromResult(ProcessChannelMessages(kickPusherClient)));
        }
    }

    private async Task ProcessChannelMessages(IKickPusherClient kickPusherClient)
    {
        await kickPusherClient.ConnectAsync();

        var connectionRequest = new KickChatConnectionRequest(
            kickPusherClient.ChannelId,
            PusherEvent.Subscribe);

        await kickPusherClient.SubscribeAsync(connectionRequest);

        var ms = new MemoryStream();
        var reader = new StreamReader(ms, Encoding.UTF8);
        var buffer = new byte[1 * 1024];

        while (true)
        {
            var result = await kickPusherClient.ReceiveAsync(buffer, CancellationToken.None);

            ms.Write(buffer, 0, result.Count);
            ms.Seek(0, SeekOrigin.Begin);

            var data = await reader.ReadToEndAsync();
            Console.WriteLine("data + " + data);

            var kickEvent = JsonSerializer.Deserialize<KickEvent>(data);

            await HandleEvent(kickEvent.Event, data);
            Console.WriteLine(data);

            ms.SetLength(0); // Clear the MemoryStream
            ms.Seek(0, SeekOrigin.Begin);
        }
    }

    private async Task HandleEvent(string pusherEvent, string data)
    {
        if (pusherEvent == PusherEvent.ChatMessage.Event)
        {
            await producerService.SendMessage(data);
        }
        else if (pusherEvent == PusherEvent.Pong.Event)
        {
            Console.WriteLine("PONG");
        }
        else if (pusherEvent == PusherEvent.Connected.Event)
        {
            Console.WriteLine("Connected...");
        }
        else if (pusherEvent == PusherEvent.Subscribed.Event)
        {
            Console.WriteLine("Listening...");
        }
    }
}
