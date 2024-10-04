namespace Wsrc.Infrastructure.Interfaces;

public interface IKickPusherClientManager
{
    public List<IKickPusherClient> ActiveConnections { get; }

    public IKickPusherClient GetClient(string channelId);

    public Task Launch();
}
