using Wsrc.Domain;
using Wsrc.Domain.Models.Chatrooms;

namespace Wsrc.Api.Business.Services.Mappings;

public static class ChatroomMapper
{
    public static IEnumerable<ChatroomDto> ToDtoList(IEnumerable<Chatroom> chatrooms)
    {
        return chatrooms.Select(ToDto);
    }
    
    public static ChatroomDto ToDto(Chatroom chatroom)
    {
        return new ChatroomDto
        {
            Id = chatroom.Id,
            Username = chatroom.Username
        };
    }
}