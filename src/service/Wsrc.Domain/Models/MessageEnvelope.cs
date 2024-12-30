namespace Wsrc.Domain.Models;

public class MessageEnvelope
{
    public object Payload { get; set; } = null!;

    public Dictionary<string, string> Headers { get; set; } = [];
}