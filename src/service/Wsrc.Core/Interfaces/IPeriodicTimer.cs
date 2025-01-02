namespace Wsrc.Core.Interfaces;

public interface IPeriodicTimer
{
    public void Initialize(TimeSpan period);

    public Task<bool> WaitForNextTickAsync(CancellationToken stoppingToken);
}