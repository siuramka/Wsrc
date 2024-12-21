using Microsoft.Extensions.DependencyInjection;
using Wsrc.Core.Interfaces;

namespace Wsrc.Core.Services.Kick;

public class KickProducerFacade(
    IKickPusherClientManager clientManager,
    IServiceScopeFactory serviceScopeFactory)
    : IKickProducerFacade
{
    public async Task HandleMessages()
    {
        await clientManager.Launch();

        foreach (var kickPusherClient in clientManager.ActiveConnections)
        {
            _ = Task.Run(() => StartProcessing(kickPusherClient));
        }
    }

    private async Task StartProcessing(IKickPusherClient kickPusherClient)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var messageProcessor = scope.ServiceProvider.GetService<IKickMessageProducerProcessor>()
                               ?? throw new NullReferenceException();
        await messageProcessor.ProcessChannelMessagesAsync(kickPusherClient);
    }
}
