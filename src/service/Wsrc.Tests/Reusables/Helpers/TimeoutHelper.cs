namespace Wsrc.Tests.Reusables.Helpers;

public static class TimeoutHelper
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(250);
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(600);

    public static async Task WaitUntilAsync(Func<Task<bool>> conditionTask)
    {
        var timeoutToken = new CancellationTokenSource(DefaultTimeout);

        while (!await conditionTask())
        {
            if (timeoutToken.IsCancellationRequested)
            {
                throw new TimeoutException($"{conditionTask.Method.Name} was timed out.");
            }

            await Task.Delay(PollInterval, timeoutToken.Token);
        }
    }

    public static async Task WaitUntilAsync(Func<bool> condition)
    {
        var timeoutToken = new CancellationTokenSource(DefaultTimeout);

        while (!condition())
        {
            if (timeoutToken.IsCancellationRequested)
            {
                throw new TimeoutException($"{condition.Method.Name} was timed out.");
            }

            await Task.Delay(PollInterval, timeoutToken.Token);
        }
    }
}