using Wsrc.Domain.Models;

namespace Wsrc.Tests.Reusables.Providers;

public class KickChatMessageBadgeProvider : ProviderBase<KickChatMessageBadge>
{
    protected override KickChatMessageBadge Entity { get; } = new() { Type = "badge", Text = "badge text", Count = 1, };
}