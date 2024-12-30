using Wsrc.Core.Interfaces.Mappings;
using Wsrc.Domain.Entities;
using Wsrc.Domain.Models;

using Message = Wsrc.Domain.Entities.Message;

namespace Wsrc.Infrastructure.Mappings;

public class KickChatMessageMapper : IKickChatMessageMapper
{
    public Sender ToSender(KickChatMessage kickChatMessage)
    {
        return new Sender
        {
            Id = kickChatMessage.Data.KickChatMessageSender.Id,
            Username = kickChatMessage.Data.KickChatMessageSender.Username.ToLower(),
            Slug = kickChatMessage.Data.KickChatMessageSender.Slug.ToLower(),
        };
    }

    public Message ToMessage(KickChatMessage kickChatMessage)
    {
        return new Message
        {
            ChatroomId = kickChatMessage.Data.ChatroomId,
            Content = kickChatMessage.Data.Content,
            Timestamp = kickChatMessage.Data.CreatedAt.ToUniversalTime(),
            SenderId = kickChatMessage.Data.KickChatMessageSender.Id,
        };
    }
}