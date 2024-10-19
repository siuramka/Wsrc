namespace Wsrc.Core.Interfaces;

public interface IKickConsumerMessageProcessor
{
    public Task ConsumeAsync(string data);
}