namespace Wsrc.Core.Interfaces;

public interface IActiveKickPusherClientFactory
{
    public Task<IEnumerable<IKickPusherClient>> CreateDisconnectedClientsAsync();

    public Task<IEnumerable<IKickPusherClient>> CreateAllClientsAsync();
}