using System.Text;
using System.Text.Json;
using Wsrc.Domain;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services.Kick;

public class KickProducerFacade(
    IKickEventStrategyHandler kickEventStrategyHandler,
    IKickPusherClientManager clientManager) : IKickProducerFacede
{
    public async Task HandleMessages()
    {
        await clientManager.Launch();

        foreach (var kickPusherClient in clientManager.ActiveConnections)
        {
            _ = Task.Run(() => ProcessChannelMessages(kickPusherClient));
        }
    }

    private async Task ProcessChannelMessages(IKickPusherClient kickPusherClient)
    {
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

            var kickEvent = JsonSerializer.Deserialize<KickEvent>(data) ?? throw new InvalidOperationException();
            var pusherEvent = PusherEvent.Parse(kickEvent.Event);

            var handler = kickEventStrategyHandler.GetStrategy(pusherEvent);
            await handler.ExecuteAsync(data);

            ms.SetLength(0);
            ms.Seek(0, SeekOrigin.Begin);
        }
    }
}
