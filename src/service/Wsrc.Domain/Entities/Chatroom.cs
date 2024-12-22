namespace Wsrc.Domain.Entities;

public class Chatroom : EntityBase
{
    public int Id { get; set; }

    public string Username { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = null!;
}