using Wsrc.Domain;
using Wsrc.Domain.Entities;
using Wsrc.Domain.Models;

namespace Wsrc.Core.Interfaces.Mappings;

public interface IKickChatMessageMapper
{
    Sender ToSender(KickChatMessage kickChatMessage);

    Message ToMessage(KickChatMessage kickChatMessage);
}