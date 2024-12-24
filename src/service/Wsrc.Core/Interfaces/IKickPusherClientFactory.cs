using Wsrc.Domain.Entities;

namespace Wsrc.Core.Interfaces;

public interface IKickPusherClientFactory
{
    IKickPusherClient CreateClient(Channel channel);

    public IEnumerable<IKickPusherClient> CreateClients(IEnumerable<Channel> channels);
}