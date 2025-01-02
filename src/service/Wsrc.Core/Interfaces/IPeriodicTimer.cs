namespace Wsrc.Core.Interfaces;

public interface IPeriodicTimer
{
    public void InitializeAsync(TimeSpan period);

    public Task<bool> WaitForNextTickAsync(CancellationToken stoppingToken);
}