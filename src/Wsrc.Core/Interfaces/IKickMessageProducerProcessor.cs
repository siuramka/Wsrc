namespace Wsrc.Core.Interfaces;

public interface IKickMessageProducerProcessor
{
    public Task ProcessChannelMessagesAsync(IKickPusherClient kickPusherClient);
}
