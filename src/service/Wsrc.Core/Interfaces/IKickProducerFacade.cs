namespace Wsrc.Core.Interfaces;

public interface IKickProducerFacade
{
    public Task InitializeAsync();

    public Task HandleReconnectAsync();
}