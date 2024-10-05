using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Wsrc.Core.Interfaces;
using Wsrc.Domain;

namespace Wsrc.Core.Services.Kick;

public class KickProducerFacade(
    IKickPusherClientManager clientManager,
    IServiceScopeFactory serviceScopeFactory)
    : IKickProducerFacede
{
    public async Task HandleMessages()
    {
        await clientManager.Launch();

        foreach (var kickPusherClient in clientManager.ActiveConnections)
        {
            _ = Task.Run(() => StartProcessingScoped(kickPusherClient));
        }
    }

    private async Task StartProcessingScoped(IKickPusherClient kickPusherClient)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var messageProcessor = scope.ServiceProvider.GetService<IKickChatChannelMessageProcessor>()
                               ?? throw new NullReferenceException();
        await messageProcessor.ProcessChannelMessagesAsync(kickPusherClient);
    }
}
