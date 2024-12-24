using Wsrc.Core.Interfaces;

namespace Wsrc.Producer.Services;

public class ProducerWorkerService(IKickProducerFacade kickProducerFacade)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await kickProducerFacade.HandleMessages();
    }
}