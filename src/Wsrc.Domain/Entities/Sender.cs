namespace Wsrc.Domain.Entities;

public class Sender : EntityBase, IEquatable<Sender>
{
    public int Id { get; set; }

    public string Username { get; set; }

    public string Slug { get; set; }

    public bool Equals(Sender? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Sender)obj);
    }

    public override int GetHashCode()
    {
        return Id;
    }

    public static bool operator ==(Sender? left, Sender? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Sender? left, Sender? right)
    {
        return !Equals(left, right);
    }
}