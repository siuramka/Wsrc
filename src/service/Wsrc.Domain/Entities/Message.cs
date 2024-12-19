namespace Wsrc.Domain.Entities;

public class Message : EntityBase
{
    public int Id { get; set; }

    public int ChatroomId { get; set; }

    public Chatroom Chatroom { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public int SenderId { get; set; }

    public Sender Sender { get; set; } = null!;
}