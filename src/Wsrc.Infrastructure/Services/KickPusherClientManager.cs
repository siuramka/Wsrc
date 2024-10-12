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

    public async Task Launch()
    {
        var kickPusherClients = pusherClientFactory.CreateClients(kick.Value.Channels);

        foreach (var kickPusherClient in kickPusherClients)
        {
            await Task.Run(() => CreateConnection(kickPusherClient));
        }
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

    public IKickPusherClient GetClient(string channelId)
    {
        return ActiveConnections.First(c => c.ChannelId == channelId);
    }
}