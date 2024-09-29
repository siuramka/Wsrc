using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Consumer;

public class ConsumerWorkerService(IConsumerService consumerService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await consumerService.ReadMessages();
    }
}