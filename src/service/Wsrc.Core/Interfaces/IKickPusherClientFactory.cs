using Wsrc.Domain;

namespace Wsrc.Core.Interfaces;

public interface IKickPusherClientFactory
{
    IKickPusherClient CreateClient(KickChannel channel);

    public IEnumerable<IKickPusherClient> CreateClients(IEnumerable<KickChannel> channels);
}
