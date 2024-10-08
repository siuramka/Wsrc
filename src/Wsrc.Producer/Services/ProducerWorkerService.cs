using Wsrc.Core.Interfaces;

namespace Wsrc.Producer.Services;

public class ProducerWorkerService(
    ILogger<ProducerWorkerService> logger,
    IKickProducerFacede kickProducerFacede)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await kickProducerFacede.HandleMessages();
    }
}
