namespace Wsrc.Core.Interfaces;

public interface IProducerService
{
    public Guid Id { get; set; }
    Task SendMessage(string message);
}
