using Microsoft.Extensions.Options;
using Wsrc.Domain;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services.Kick;

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
