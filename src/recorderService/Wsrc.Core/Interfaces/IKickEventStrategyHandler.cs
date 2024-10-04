using Wsrc.Domain;

namespace Wsrc.Core.Interfaces;

public interface IKickEventStrategyHandler
{
    public IKickEventStrategy GetStrategy(PusherEvent pusherEvent);
}
