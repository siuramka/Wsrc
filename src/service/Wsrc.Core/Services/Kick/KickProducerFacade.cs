using Microsoft.Extensions.DependencyInjection;

using Wsrc.Core.Interfaces;

namespace Wsrc.Core.Services.Kick;

public class KickProducerFacade(
    IKickPusherClientManager clientManager,
    IActiveClientsManager activeClientsManager,
    IServiceScopeFactory serviceScopeFactory)
    : IKickProducerFacade
{
    private readonly List<IKickPusherClient> _producingClients = [];

    private readonly Lock _lock = new();

    public async Task InitializeAsync()
    {
        await clientManager.LaunchAsync();
        LaunchClientMessageProcessors();
    }

    public async Task HandleReconnectAsync()
    {
        await clientManager.ReconnectAsync();
        LaunchClientMessageProcessors();
    }

    public void HandleDisconnect(int channelId)
    {
        lock (_lock)
        {
            var client = _producingClients.First(c => c.ChannelId == channelId);

            _producingClients.Remove(client);
        }
    }

    private void LaunchClientMessageProcessors()
    {
        var clients = activeClientsManager.GetActiveClients();

        var newClients = clients
            .Where(client => !_producingClients.Contains(client))
            .ToList();

        _ = newClients.Select(client => Task.Run(() => StartProcessingAsync(client))).ToList();
    }

    private async Task StartProcessingAsync(IKickPusherClient kickPusherClient)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var messageProcessor = scope.ServiceProvider.GetService<IKickMessageProducerProcessor>()
                               ?? throw new NullReferenceException();

        lock (_lock)
        {
            _producingClients.Add(kickPusherClient);
        }

        await messageProcessor.ProcessChannelMessagesAsync(kickPusherClient);

        lock (_lock)
        {
            _producingClients.Remove(kickPusherClient);
        }
        
        await clientManager.HandleDisconnectAsync(kickPusherClient.ChannelId);
    }
}