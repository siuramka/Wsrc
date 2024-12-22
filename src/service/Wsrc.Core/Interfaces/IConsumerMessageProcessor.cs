namespace Wsrc.Core.Interfaces;

public interface IConsumerMessageProcessor
{
    public Task ConsumeAsync(string data);
}