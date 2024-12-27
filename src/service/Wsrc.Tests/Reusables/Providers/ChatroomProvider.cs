using Wsrc.Domain.Entities;

namespace Wsrc.Tests.Reusables.Providers;

public class ChatroomProvider
{
    public List<Chatroom> ProvideDefault()
    {
        return
        [
            new Chatroom { Id = 11111, Username = "TestChannel1", },
            new Chatroom { Id = 22222, Username = "TestChannel2", },
        ];
    }
}