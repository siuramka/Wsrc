namespace Wsrc.Core.Interfaces;

public interface IKickChatChannelMessageProcessor
{
    public Task ProcessChannelMessagesAsync(IKickPusherClient kickPusherClient);
}
