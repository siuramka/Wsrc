using Wsrc.Domain.Models;

namespace Wsrc.Tests.Reusables.Providers;

public class KickChatMessageSenderProvider : ProviderBase<KickChatMessageSender>
{
    protected override KickChatMessageSender Entity { get; } = new()
    {
        Id = 215425,
        Username = "User1",
        Slug = "user1",
        KickChatMessageIdentity = new KickChatMessageIdentityProvider().Create(),
    };
}