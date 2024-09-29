using RabbitMQ.Client;

namespace Wsrc.Infrastructure.Interfaces;

public interface IRabbitMqService
{
    public Task<IConnection> CreateConnectionAsync();
}