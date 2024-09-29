using Microsoft.Extensions.Options;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services.Kick;

public class KickPusherClientFactory(IOptions<KickConfiguration> kickConfiguration) : IKickPusherClientFactory
{
    public IKickPusherClient CreateClient(string chatroomId)
    {
        return new KickPusherClient(kickConfiguration)
        {
            ChatRoomId = chatroomId
        };
    }

    public IEnumerable<IKickPusherClient> CreateClients(IEnumerable<string> chatroomIds)
    {
        return chatroomIds.Select(CreateClient);
    }
}
