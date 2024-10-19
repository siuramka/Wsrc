using Microsoft.EntityFrameworkCore;
using Wsrc.Api.Business.Mappings;
using Wsrc.Domain;
using Wsrc.Domain.Models.Chatrooms;
using Wsrc.Domain.Repositories;

namespace Wsrc.Api.Business.Services;

public class ChatroomService(ChatroomRepository chatroomRepository)
{
    public async Task<IEnumerable<ChatroomDto>> GetAllAsync(string? channelName)
    {
        var chatrooms = await chatroomRepository.GetAllAsync(channelName);

        return ChatroomMapper.ToDtoList(chatrooms);
    }
}