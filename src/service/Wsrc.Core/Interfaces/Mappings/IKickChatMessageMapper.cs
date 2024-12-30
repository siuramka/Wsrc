using Wsrc.Domain.Entities;
using Wsrc.Domain.Models;

using Message = Wsrc.Domain.Entities.Message;

namespace Wsrc.Core.Interfaces.Mappings;

public interface IKickChatMessageMapper
{
    Sender ToSender(KickChatMessage kickChatMessage);

    Message ToMessage(KickChatMessage kickChatMessage);
}