using Wsrc.Core.Interfaces;

namespace Wsrc.Producer.Services;

public class ProducerWorkerService(
    IKickProducerFacade kickProducerFacade,
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    private readonly TimeSpan _reconnectPeriod = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var periodicTimer = scope.ServiceProvider.GetRequiredService<IPeriodicTimer>();

        periodicTimer.Initialize(_reconnectPeriod);

        await kickProducerFacade.InitializeAsync();

        while (await periodicTimer.WaitForNextTickAsync(stoppingToken)
               && !stoppingToken.IsCancellationRequested)
        {
            await kickProducerFacade.HandleReconnectAsync();
        }
    }
}