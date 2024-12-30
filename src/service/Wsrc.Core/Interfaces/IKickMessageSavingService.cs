using Wsrc.Domain.Models;

namespace Wsrc.Core.Interfaces;

public interface IKickMessageSavingService
{
    public Task HandleMessageAsync(ParsedKickChatMessage parsedKickChatMessage);
}