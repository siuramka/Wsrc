using Wsrc.Infrastructure.Persistence;
using Wsrc.Tests.Reusables.Providers;

namespace Wsrc.Tests.Integration.Reusables.Helpers;

public class DefaultDataSeeder
{
    public async Task SeedRequiredDataAsync(WsrcContext context)
    {
        var channels = new ChannelProvider().ProvideDefault();
        var chatrooms = new ChatroomProvider().ProvideDefault();

        await context.Channels.AddRangeAsync(channels);
        await context.Chatrooms.AddRangeAsync(chatrooms);

        await context.SaveChangesAsync();
    }
}