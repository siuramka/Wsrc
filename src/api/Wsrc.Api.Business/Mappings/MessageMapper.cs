using Wsrc.Domain;
using Wsrc.Domain.Models.Chatmessages;
using Wsrc.Domain.Models.Chatrooms;

namespace Wsrc.Api.Business.Mappings;

public static class MessageMapper
{
    public static IEnumerable<MessageDto> ToDtoList(IEnumerable<Message> messages)
    {
        return messages.Select(ToDto);
    }

    public static MessageDto ToDto(Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            Timestamp = message.Timestamp,
            Content = message.Content
        };
    }
}