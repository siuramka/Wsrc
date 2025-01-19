using Wsrc.Core.Interfaces;
using Wsrc.Domain.Models;

namespace Wsrc.Infrastructure.Services;

public class KickPusherClientManager(
    IActiveKickPusherClientFactory kickPusherClientFactory,
    IActiveClientsManager activeClientsManager) 
    : IKickPusherClientManager
{
    public async Task LaunchAsync()
    {
        var kickPusherClients = await kickPusherClientFactory.CreateAllClientsAsync();

        foreach (var kickPusherClient in kickPusherClients)
        {
            await CreateConnectionAsync(kickPusherClient);
        }
    }

    public async Task ReconnectAsync()
    {
        var kickPusherClients = await kickPusherClientFactory.CreateDisconnectedClientsAsync();

        foreach (var kickPusherClient in kickPusherClients)
        {
            await CreateConnectionAsync(kickPusherClient);
        }
    }

    public async Task HandleDisconnectAsync(int channelId)
    {
        await activeClientsManager.RemoveAsync(channelId);
    }

    private async Task CreateConnectionAsync(IKickPusherClient kickPusherClient)
    {
        await kickPusherClient.ConnectAsync();

        var connectionRequest = new KickChatConnectionRequest(
            kickPusherClient.ChannelId,
            PusherEvent.Subscribe);

        await kickPusherClient.SubscribeAsync(connectionRequest);

        await activeClientsManager.AddAsync(kickPusherClient);
    }
}