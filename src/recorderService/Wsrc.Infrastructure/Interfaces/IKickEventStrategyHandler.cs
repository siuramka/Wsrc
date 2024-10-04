using Wsrc.Domain;

namespace Wsrc.Infrastructure.Interfaces;

public interface IKickEventStrategyHandler
{
    public IKickEventStrategy GetStrategy(PusherEvent pusherEvent);
}
