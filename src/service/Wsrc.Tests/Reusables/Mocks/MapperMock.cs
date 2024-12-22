using NSubstitute;
using Wsrc.Core.Interfaces.Mappings;

namespace Wsrc.Tests.Reusables.Mocks;

public class MapperMock
{
    public IMapper SetupMapper()
    {
        var mapper = Substitute.For<IMapper>();

        mapper.KickChatMessageMapper.Returns(Substitute.For<IKickChatMessageMapper>());

        return mapper;
    }
}
