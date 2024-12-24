using Wsrc.Domain.Models;

namespace Wsrc.Core.Interfaces;

public interface IKickEventStrategyHandler
{
    public IKickEventStrategy GetStrategy(PusherEvent pusherEvent);
}