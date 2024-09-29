namespace Wsrc.Infrastructure.Interfaces;

public interface IKickPusherClientFactory
{
    IKickPusherClient CreateClient(string chatroomId);

    public IEnumerable<IKickPusherClient> CreateClients(IEnumerable<string> chatroomIds);
}