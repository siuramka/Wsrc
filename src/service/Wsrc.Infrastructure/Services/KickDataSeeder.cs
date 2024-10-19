using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Domain.Entities;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services;

public class KickDataSeeder(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<KickConfiguration> kickConfig)
    : IKickDataSeeder
{
    public async Task SeedData()
    {
        await SeedChatrooms();
    }

    private async Task SeedChatrooms()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var chatroomRepository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<Chatroom>>();

        foreach (var channel in kickConfig.Value.Channels)
        {
            var chatroom = await chatroomRepository.FirstOrDefaultAsync(c => c.Id == channel.Id);
            if (chatroom is not null)
            {
                continue;
            }

            var newChatroom = new Chatroom
            {
                Id = channel.Id,
                Username = channel.Name,
            };

            await chatroomRepository.AddAsync(newChatroom);
        }
    }
}