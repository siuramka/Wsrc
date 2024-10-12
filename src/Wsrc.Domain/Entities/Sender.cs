namespace Wsrc.Domain.Entities;

public class Sender : EntityBase
{
    public int Id { get; set; }
    
    public string Username { get; set; }
    
    public string Slug { get; set; }
}