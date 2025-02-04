namespace Wsrc.Core.Interfaces;

public interface IKickPusherClientManager
{
    public Task LaunchAsync();

    public Task ReconnectAsync();

    public Task HandleDisconnectAsync(int channelId);
}