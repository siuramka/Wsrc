using Microsoft.EntityFrameworkCore;
using Wsrc.Domain;
using Wsrc.Domain.Repositories;

namespace Wsrc.Api.Business.Services;

public class ChatroomService(WsrcDbContext dbContext)
{
    public async Task<IEnumerable<Chatroom>> GetAllAsync(string channelName)
    {
        if (string.IsNullOrWhiteSpace(channelName))
        {
            return await dbContext.Chatrooms.ToListAsync();
        }

        return await dbContext.Chatrooms
            .Where(c => c.Username.Contains(channelName.ToLower()))
            .ToListAsync();
    }
}