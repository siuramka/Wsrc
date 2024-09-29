using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services;

public class RabbitMqService(IOptions<RabbitMqConfiguration> options) : IRabbitMqService
{
    private readonly RabbitMqConfiguration _configuration = options.Value;

    public async Task<IConnection> CreateConnectionAsync()
    {
        var defaultConnection = new ConnectionFactory()
        {
            UserName = _configuration.Username,
            Password = _configuration.Password,
            HostName = _configuration.HostName
        };

        var connection = await defaultConnection.CreateConnectionAsync();
        return connection;
    }
}