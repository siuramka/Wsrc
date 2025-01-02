namespace Wsrc.Core.Interfaces;

public interface IKickPusherClientManager
{
    public IEnumerable<IKickPusherClient> GetActiveClients();

    public Task LaunchAsync();

    public Task ReconnectAsync();
}