using Wsrc.Domain;

namespace Wsrc.Core.Interfaces;

public interface IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent);
    public Task ExecuteAsync(string data);
}