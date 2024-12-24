namespace Wsrc.Core.Interfaces;

public interface IProducerService
{
    Task SendMessage(string message);
}