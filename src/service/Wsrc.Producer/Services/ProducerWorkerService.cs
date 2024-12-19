using Wsrc.Core.Interfaces;

namespace Wsrc.Producer.Services;

public class ProducerWorkerService(
    ILogger<ProducerWorkerService> logger,
    IKickProducerFacede kickProducerFacade)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await kickProducerFacade.HandleMessages();
    }
}
