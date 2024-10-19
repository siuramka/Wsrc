using Microsoft.EntityFrameworkCore;
using Wsrc.Api.Business.Services.Mappings;
using Wsrc.Domain;
using Wsrc.Domain.Models.Chatrooms;
using Wsrc.Domain.Repositories;

namespace Wsrc.Api.Business.Services;

public class ChatroomService(WsrcDbContext dbContext)
{
    public async Task<IEnumerable<ChatroomDto>> GetAllAsync(string channelName)
    {
        var chatroomsQuery = dbContext.Chatrooms.AsQueryable();

        if (!string.IsNullOrWhiteSpace(channelName))
        {
            chatroomsQuery = chatroomsQuery.Where(c => EF.Functions.Like(c.Username, $"%{channelName}%"));
        }

        var chatrooms = await chatroomsQuery.ToListAsync();

        return ChatroomMapper.ToDtoList(chatrooms);
    }
}