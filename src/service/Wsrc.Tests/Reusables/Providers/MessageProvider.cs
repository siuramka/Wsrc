using Wsrc.Domain.Entities;

namespace Wsrc.Tests.Reusables.Providers;

public class MessageProvider : ProviderBase<Message>
{
    protected override Message Entity { get; } = new()
    {
        Id = 325325325,
        Content = "TestContent",
        Timestamp = new DateTime(2022, 01, 05),
    };
}