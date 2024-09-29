using Microsoft.Extensions.Options;
using Wsrc.Domain;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Producer.Services;

public class ProducerWorkerService(
    ILogger<ProducerWorkerService> logger,
    IOptions<KickConfiguration> kick,
    IKickPusherClientFactory pusherClientFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //listen to websockets
        // when get message
        // parse, send to producer
        var kickPusherClients = pusherClientFactory.CreateClients(kick.Value.Channels);

        foreach (var kickPusherClient in kickPusherClients)
        {
            var connectionRequest = new KickChatConnectionRequest(kickPusherClient.ChatRoomId, PusherEvent.Subscribe);
            await kickPusherClient.ConnectAsync(connectionRequest);
        }
    }
}
