using Wsrc.Core.Interfaces.Mappings;

namespace Wsrc.Infrastructure.Mappings;

public class Mapper(IKickChatMessageMapper kickChatMessageMapper) : IMapper
{
    public IKickChatMessageMapper KickChatMessageMapper { get; init; } = kickChatMessageMapper;
}
