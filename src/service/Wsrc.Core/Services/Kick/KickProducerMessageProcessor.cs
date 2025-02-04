using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using Wsrc.Core.Interfaces;
using Wsrc.Domain;
using Wsrc.Domain.Models;

namespace Wsrc.Core.Services.Kick;

public class KickProducerMessageProcessor(IKickEventStrategyHandler eventStrategyHandler)
    : IKickMessageProducerProcessor
{
    public async Task ProcessChannelMessagesAsync(IKickPusherClient kickPusherClient)
    {
        var ms = new MemoryStream();
        var reader = new StreamReader(ms, Encoding.UTF8);
        var buffer = new byte[1 * 1024];

        while (true)
        {
            var result = await kickPusherClient.ReceiveAsync(buffer, CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                return;
            }

            await ms.WriteAsync(buffer.AsMemory(0, result.Count));
            ms.Seek(0, SeekOrigin.Begin);

            var data = await reader.ReadToEndAsync();

            var message = new MessageEnvelope
            {
                Payload = data,
            };

            var kickEvent = JsonSerializer.Deserialize<KickEvent>(data);
            var pusherEvent = PusherEvent.Parse(kickEvent!.Event);

            var handler = eventStrategyHandler.GetStrategy(pusherEvent);

            await handler.ExecuteAsync(message);

            ms.SetLength(0);
            ms.Seek(0, SeekOrigin.Begin);
        }
    }
}