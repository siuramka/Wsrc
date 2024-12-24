using Wsrc.Domain;

namespace Wsrc.Tests.Reusables.Providers;

public class KickChatMessageIdentityProvider : ProviderBase<KickChatMessageIdentity>
{
    protected override KickChatMessageIdentity Entity { get; } = new()
    {
        Color = "#FFFFFF",
        Badges = new KickChatMessageBadgeProvider().CreateList(5),
    };
}