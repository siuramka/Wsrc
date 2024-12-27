namespace Wsrc.Tests.Reusables.Helpers;

public static class TimeoutHelper
{
    public static async Task WaitUntilAsync(
        Func<Task<bool>> conditionTask,
        TimeSpan pollInterval,
        CancellationToken ct)
    {
        while (!await conditionTask())
        {
            if (ct.IsCancellationRequested)
            {
                throw new TimeoutException("WaitUntil task was timed out.");
            }

            await Task.Delay(pollInterval, ct);
        }
    }
}