using Wsrc.Domain.Entities;

namespace Wsrc.Tests.Reusables.Providers;

public class ChannelProvider
{
    public List<Channel> ProvideDefault()
    {
        return
        [
            new Channel { Id = 11111, Name = "TestChannel1", },
            new Channel { Id = 22222, Name = "TestChannel2", },
        ];
    }
}