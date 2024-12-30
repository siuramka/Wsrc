namespace Wsrc.Domain.Models;

public class ParsedKickChatMessage
{
    public required KickChatMessage KickChatMessage { get; set; }

    public required MessageEnvelope MessageEnvelope { get; set; }
}