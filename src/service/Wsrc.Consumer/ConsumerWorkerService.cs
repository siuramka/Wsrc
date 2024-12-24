using Wsrc.Core.Interfaces;

namespace Wsrc.Consumer;

public class ConsumerWorkerService(IConsumerService consumerService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await consumerService.ConnectAsync();
        await consumerService.ConsumeMessagesAsync();
    }
}