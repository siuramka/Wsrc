using Wsrc.Domain.Models;

namespace Wsrc.Tests.Reusables.Providers;

public class KickChatMessageChatInfoProvider : ProviderBase<KickChatMessageChatInfo>
{
    protected override KickChatMessageChatInfo Entity { get; } = new()
    {
        Id = Guid.NewGuid(),
        ChatroomId = 1000,
        Content = "Test content",
        Type = "Test type",
        CreatedAt = new DateTime(2022, 02, 02),
        KickChatMessageSender = new KickChatMessageSenderProvider().Create(),
    };
}