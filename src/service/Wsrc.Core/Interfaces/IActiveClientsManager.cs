namespace Wsrc.Core.Interfaces;

public interface IActiveClientsManager
{
    public IEnumerable<IKickPusherClient> GetActiveClients();

    public Task RemoveAsync(int channelId);

    public Task AddAsync(IKickPusherClient client);
}