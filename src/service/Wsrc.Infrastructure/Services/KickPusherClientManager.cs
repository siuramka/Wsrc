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
    private readonly List<IKickPusherClient> _activeConnections = [];

    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    private readonly Lock _lock = new();

    public async Task LaunchAsync()
    {
        var kickPusherClients = await CreateClientsAsync();

        foreach (var kickPusherClient in kickPusherClients)
        {
            await CreateConnectionAsync(kickPusherClient);
        }
    }

    public async Task ReconnectAsync()
    {
        var kickPusherClients = await CreateDisconnectedClientsAsync();

        foreach (var kickPusherClient in kickPusherClients)
        {
            await CreateConnectionAsync(kickPusherClient);
        }
    }

    public IEnumerable<IKickPusherClient> GetActiveClients()
    {
        return _activeConnections;
    }

    public async Task HandleDisconnectAsync(int channelId)
    {
        await _semaphoreSlim.WaitAsync();

        try
        {
            var client = _activeConnections.First(c => c.ChannelId == channelId);
            _activeConnections.Remove(client);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private async Task<IEnumerable<IKickPusherClient>> CreateDisconnectedClientsAsync()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var channelRepository = scope.ServiceProvider
                                    .GetService<IAsyncRepository<Channel>>()
                                ?? throw new NullReferenceException();

        var activeChannelsIds = _activeConnections
            .Select(ac => ac.ChannelId)
            .ToList();

        var inactiveChannels = await channelRepository
            .GetWhereAsync(c => !activeChannelsIds.Contains(c.Id));

        var newKickPusherClients = pusherClientFactory
            .CreateClients(inactiveChannels.ToList());

        return newKickPusherClients;
    }

    private async Task<IEnumerable<IKickPusherClient>> CreateClientsAsync()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var channelRepository = scope.ServiceProvider
                                    .GetService<IAsyncRepository<Channel>>()
                                ?? throw new NullReferenceException();

        var channels = await channelRepository.GetAllAsync();

        var kickPusherClients = pusherClientFactory.CreateClients(channels.ToList());
        return kickPusherClients;
    }

    private async Task CreateConnectionAsync(IKickPusherClient kickPusherClient)
    {
        await kickPusherClient.ConnectAsync();

        var connectionRequest = new KickChatConnectionRequest(
            kickPusherClient.ChannelId,
            PusherEvent.Subscribe);

        await kickPusherClient.SubscribeAsync(connectionRequest);

        lock (_lock)
        {
            _activeConnections.Add(kickPusherClient);
        }
    }
}