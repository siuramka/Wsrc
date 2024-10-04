using Wsrc.Domain;

namespace Wsrc.Infrastructure.Interfaces;

public interface IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent);

    public Task ExecuteAsync(string messageData);
}
