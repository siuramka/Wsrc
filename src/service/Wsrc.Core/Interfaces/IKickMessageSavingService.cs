using Wsrc.Domain;

namespace Wsrc.Core.Interfaces;

public interface IKickMessageSavingService
{
    public Task HandleMessageAsync(KickChatMessage kickChatMessage);
}