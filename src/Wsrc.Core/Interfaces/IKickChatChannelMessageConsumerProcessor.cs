namespace Wsrc.Core.Interfaces;

public interface IKickChatChannelMessageConsumerProcessor
{
    public Task ConsumeAsync(string data);
}