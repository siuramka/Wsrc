using Wsrc.Core.Interfaces;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services;

public class ActiveClientsManager : IActiveClientsManager
{
    private readonly List<IKickPusherClient> _activeConnections = [];

    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public IEnumerable<IKickPusherClient> GetActiveClients()
    {
        return _activeConnections;
    }

    public async Task RemoveAsync(int channelId)
    {
        await _semaphoreSlim.WaitAsync();

        try
        {
            var client = _activeConnections.First(c => c.ChannelId == channelId);
            _activeConnections.Remove(client);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
    
    public async Task AddAsync(IKickPusherClient client)
    {
        await _semaphoreSlim.WaitAsync();

        try
        {
            _activeConnections.Add(client);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}

