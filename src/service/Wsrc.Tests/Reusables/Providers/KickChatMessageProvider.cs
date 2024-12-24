using Wsrc.Domain.Models;

namespace Wsrc.Tests.Reusables.Providers;

public class KickChatMessageProvider : ProviderBase<KickChatMessage>
{
    protected override KickChatMessage Entity { get; } = new()
    {
        Event = PusherEvent.ChatMessage.Event,
        Data = new KickChatMessageChatInfoProvider().Create(),
    };
}