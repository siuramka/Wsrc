using Wsrc.Domain;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services.Kick.EventStrategies;

public class KickEventStrategyHandler(IEnumerable<IKickEventStrategy> eventStrategies) : IKickEventStrategyHandler
{
    public IKickEventStrategy GetStrategy(PusherEvent pusherEvent)
    {
        return eventStrategies.First(ikcs => ikcs.IsApplicable(pusherEvent));
    }
}
