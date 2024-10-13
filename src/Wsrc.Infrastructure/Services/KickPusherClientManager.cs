using Microsoft.Extensions.Options;
using Wsrc.Core.Interfaces;
using Wsrc.Domain;
using Wsrc.Infrastructure.Configuration;

namespace Wsrc.Infrastructure.Services;

public class KickPusherClientManager(
    IKickPusherClientFactory pusherClientFactory,
    IOptions<KickConfiguration> kick) : IKickPusherClientManager
{
    public List<IKickPusherClient> ActiveConnections { get; } = [];

    public Task Launch()
    {
        var kickPusherClients = pusherClientFactory.CreateClients(kick.Value.Channels);

        foreach (var kickPusherClient in kickPusherClients)
        {
            _ = CreateConnection(kickPusherClient);
        }

        return Task.CompletedTask;
    }

    private async Task CreateConnection(IKickPusherClient kickPusherClient)
    {
        await kickPusherClient.ConnectAsync();

        var connectionRequest = new KickChatConnectionRequest(
            kickPusherClient.ChannelId,
            PusherEvent.Subscribe);

        await kickPusherClient.SubscribeAsync(connectionRequest);

        ActiveConnections.Add(kickPusherClient);
    }

    public IKickPusherClient GetClient(int channelId)
    {
        return ActiveConnections.First(c => c.ChannelId == channelId);
    }
}