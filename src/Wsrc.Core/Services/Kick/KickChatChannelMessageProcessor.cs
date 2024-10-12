using System.Text;
using System.Text.Json;
using Wsrc.Core.Interfaces;
using Wsrc.Domain;

namespace Wsrc.Core.Services.Kick;

public class KickChatChannelMessageProcessor(IKickEventStrategyHandler eventStrategyHandler)
    : IKickChatChannelMessageProcessor
{
    public async Task ProcessChannelMessagesAsync(IKickPusherClient kickPusherClient)
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

            var kickEvent = JsonSerializer.Deserialize<KickEvent>(data) ?? throw new InvalidOperationException();
            var pusherEvent = PusherEvent.Parse(kickEvent.Event);

            var handler = eventStrategyHandler.GetStrategy(pusherEvent) ?? throw new InvalidOperationException();

            await handler.ExecuteAsync(data);

            ms.SetLength(0);
            ms.Seek(0, SeekOrigin.Begin);
        }
    }
}