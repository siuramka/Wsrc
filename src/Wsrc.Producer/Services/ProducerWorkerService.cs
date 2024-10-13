using Wsrc.Core.Interfaces;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Producer.Services;

public class ProducerWorkerService(
    ILogger<ProducerWorkerService> logger,
    IKickProducerFacede kickProducerFacede,
    IKickDataSeeder kickDataSeeder)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await kickDataSeeder.SeedData(); 
        await kickProducerFacede.HandleMessages();
    }
}
