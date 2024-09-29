namespace Wsrc.Producer.Services;

public class ProducerWorkerService(
    ILogger<ProducerWorkerService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

    }
}
