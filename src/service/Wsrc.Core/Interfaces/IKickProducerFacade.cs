namespace Wsrc.Core.Interfaces;

public interface IKickProducerFacade
{
    public Task InitializeAsync();

    public Task HandleReconnectAsync();

    public void HandleDisconnect(int channelId);
}