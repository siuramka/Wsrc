using Wsrc.Core.Interfaces;
using Wsrc.Domain.Models;

namespace Wsrc.Core.Services.Kick.EventStrategies;

public class KickEventStrategyHandler(
    IEnumerable<IKickEventStrategy> eventStrategies)
    : IKickEventStrategyHandler
{
    public IKickEventStrategy GetStrategy(PusherEvent pusherEvent)
    {
        return eventStrategies.First(ikcs => ikcs.IsApplicable(pusherEvent));
    }
}
