using Microsoft.Extensions.DependencyInjection;

using Wsrc.Core.Interfaces;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Domain;
using Wsrc.Domain.Entities;
using Wsrc.Domain.Models;

namespace Wsrc.Infrastructure.Services;

public class KickPusherClientManager(
    IServiceScopeFactory serviceScopeFactory,
    IKickPusherClientFactory pusherClientFactory) : IKickPusherClientManager
{
    public List<IKickPusherClient> ActiveConnections { get; } = [];

    public async Task Launch()
    {
        var kickPusherClients = await GetClients();

        foreach (var kickPusherClient in kickPusherClients)
        {
            await CreateConnection(kickPusherClient);
        }
    }

    private async Task<IEnumerable<IKickPusherClient>> GetClients()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var channelRepository = scope.ServiceProvider
                                    .GetService<IAsyncRepository<Channel>>()
                                ?? throw new NullReferenceException();

        var channels = await channelRepository.GetAllAsync();

        var kickPusherClients = pusherClientFactory.CreateClients(channels.ToList());
        return kickPusherClients;
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