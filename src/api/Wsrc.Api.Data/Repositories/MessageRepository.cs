using Microsoft.EntityFrameworkCore;

namespace Wsrc.Domain.Repositories;

public class MessageRepository(WsrcDbContext dbContext)
{
    public async Task<IEnumerable<Message>> GetAllAsync(string? channelName, string? senderUsername)
    {
        var messagesQuery = dbContext.Messages.AsQueryable();

        if (!string.IsNullOrWhiteSpace(channelName))
        {
            messagesQuery = messagesQuery
                .Include(m => m.Chatroom)
                .Where(m =>
                    m.Chatroom.Username.ToLower()
                        .StartsWith(channelName.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(senderUsername))
        {
            messagesQuery = messagesQuery
                .Include(m => m.Sender)
                .Where(m => m.Sender.Username.ToLower()
                    .StartsWith(senderUsername.ToLower()));
        }

        return await messagesQuery.ToListAsync();
    }
}