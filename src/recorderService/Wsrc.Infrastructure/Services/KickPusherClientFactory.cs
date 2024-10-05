using Microsoft.Extensions.Options;
using Wsrc.Core.Interfaces;
using Wsrc.Domain;
using Wsrc.Infrastructure.Configuration;

namespace Wsrc.Infrastructure.Services;

public class KickPusherClientFactory(IOptions<KickConfiguration> kickConfiguration) : IKickPusherClientFactory
{
    public IKickPusherClient CreateClient(KickChannel channel)
    {
        return new KickPusherClient(kickConfiguration)
        {
            ChannelName = channel.Name,
            ChannelId = channel.Id,
        };
    }

    public IEnumerable<IKickPusherClient> CreateClients(IEnumerable<KickChannel> channels)
    {
        return channels.Select(CreateClient);
    }
}
