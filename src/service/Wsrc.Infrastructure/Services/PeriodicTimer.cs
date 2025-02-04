using Wsrc.Core.Interfaces;

namespace Wsrc.Infrastructure.Services;

public class WsrcPeriodicTimer : IPeriodicTimer
{
    private PeriodicTimer Timer { get; set; } = null!;

    public void Initialize(TimeSpan period)
    {
        Timer = new PeriodicTimer(period);
    }

    public async Task<bool> WaitForNextTickAsync(CancellationToken stoppingToken)
    {
        return await Timer.WaitForNextTickAsync(stoppingToken);
    }

    public void Dispose()
    {
        Timer.Dispose();
    }
}