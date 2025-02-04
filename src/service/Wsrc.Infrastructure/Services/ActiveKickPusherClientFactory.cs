using Microsoft.Extensions.DependencyInjection;

using Wsrc.Core.Interfaces;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Domain.Entities;

namespace Wsrc.Infrastructure.Services;

public class ActiveKickPusherClientFactory(
    IActiveClientsManager activeClientsManager,
    IServiceScopeFactory serviceScopeFactory,
    IKickPusherClientFactory pusherClientFactory)
    : IActiveKickPusherClientFactory
{
    public async Task<IEnumerable<IKickPusherClient>> CreateDisconnectedClientsAsync()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var channelRepository = scope.ServiceProvider
                                    .GetService<IAsyncRepository<Channel>>()
                                ?? throw new NullReferenceException();

        var activeChannelsIds = activeClientsManager.GetActiveClients()
            .Select(ac => ac.ChannelId)
            .ToList();

        //TODO: repository
        var inactiveChannels = await channelRepository
            .GetWhereAsync(c => !activeChannelsIds.Contains(c.Id));

        var newKickPusherClients = pusherClientFactory
            .CreateClients(inactiveChannels);

        return newKickPusherClients;
    }

    public async Task<IEnumerable<IKickPusherClient>> CreateAllClientsAsync()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var channelRepository = scope.ServiceProvider
                                    .GetService<IAsyncRepository<Channel>>()
                                ?? throw new NullReferenceException();

        var channels = await channelRepository.GetAllAsync();

        var kickPusherClients = pusherClientFactory.CreateClients(channels);
        return kickPusherClients;
    }
}