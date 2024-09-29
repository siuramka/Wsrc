namespace Wsrc.Infrastructure.Interfaces;

public interface IConsumerService
{
    Task ConnectAsync();

    Task ReadMessages();

    ValueTask DisposeAsync();
}