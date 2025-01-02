using Microsoft.Extensions.DependencyInjection;

using Wsrc.Core.Interfaces;

namespace Wsrc.Core.Services.Kick;

public class KickProducerFacade(
    IKickPusherClientManager clientManager,
    IServiceScopeFactory serviceScopeFactory)
    : IKickProducerFacade
{
    private readonly List<IKickPusherClient> _producingClients = [];

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

    private void LaunchClientMessageProcessors()
    {
        var clients = clientManager.GetActiveClients();

        var newClients = clients
            .Where(client => !_producingClients.Contains(client))
            .ToList();

        _ = newClients.Select(client => Task.Run(() => StartProcessing(client))).ToList();
    }

    private async Task StartProcessing(IKickPusherClient kickPusherClient)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var messageProcessor = scope.ServiceProvider.GetService<IKickMessageProducerProcessor>()
                               ?? throw new NullReferenceException();

        _producingClients.Add(kickPusherClient);

        await messageProcessor.ProcessChannelMessagesAsync(kickPusherClient);
    }
}