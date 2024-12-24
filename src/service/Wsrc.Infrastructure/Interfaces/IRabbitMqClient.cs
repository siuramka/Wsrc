using RabbitMQ.Client;

namespace Wsrc.Infrastructure.Interfaces;

public interface IRabbitMqClient
{
    public Task<IConnection> CreateConnectionAsync();
}