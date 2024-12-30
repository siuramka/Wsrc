using Wsrc.Domain.Models;

namespace Wsrc.Core.Interfaces;

public interface IKickEventStrategy
{
    public bool IsApplicable(PusherEvent pusherEvent);

    public Task ExecuteAsync(MessageEnvelope messageEnvelope);
}