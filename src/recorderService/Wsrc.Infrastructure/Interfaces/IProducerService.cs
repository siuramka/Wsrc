namespace Wsrc.Infrastructure.Interfaces;

public interface IProducerService
{
    Task SendMessage(string message);
}
