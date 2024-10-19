namespace Wsrc.Domain.Models.Chatmessages;

public class MessageDto
{
    public int Id { get; set; }
    
    public required string Content { get; set; }
    
    public DateTime Timestamp { get; set; }
}