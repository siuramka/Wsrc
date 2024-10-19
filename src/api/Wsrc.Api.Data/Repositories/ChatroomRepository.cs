using Microsoft.EntityFrameworkCore;

namespace Wsrc.Domain.Repositories;

public class ChatroomRepository(WsrcDbContext dbContext)
{
    public async Task<IEnumerable<Chatroom>> GetAllAsync(string? channelName)
    {
        var chatroomsQuery = dbContext.Chatrooms.AsQueryable();

        if (!string.IsNullOrWhiteSpace(channelName))
        {
            chatroomsQuery = chatroomsQuery.Where(c =>
                c.Username.ToLower()
                    .StartsWith(channelName.ToLower()));
        }

        return await chatroomsQuery.ToListAsync();
    }
}