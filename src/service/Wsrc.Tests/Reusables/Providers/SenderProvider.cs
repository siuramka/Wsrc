using Wsrc.Domain.Entities;

namespace Wsrc.Tests.Reusables.Providers;

public class SenderProvider : ProviderBase<Sender>
{
    protected override Sender Entity { get; } = new()
    {
        Id = 425146436,
        Username = "User1",
        Slug = "user",
        Messages = [],
    };
}