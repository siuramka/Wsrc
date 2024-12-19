namespace Wsrc.Core.Interfaces;

public interface IConsumerService
{
    Task ConnectAsync();

    Task ConsumeMessagesAsync();
}
