namespace Wsrc.Domain.Entities;

public class Channel : EntityBase
{
    public int Id { get; set; }

    public required string Name { get; set; }
}