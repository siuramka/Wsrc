using Wsrc.Infrastructure.Configuration;

namespace Wsrc.Tests.Integration.Reusables.Helpers;

public class TestConfigurationsSetup
{
    public Dictionary<string, string> GetRabbitMqConfig(RabbitMqConfiguration config) => new()
    {
        { "RabbitMQ:HostName", config.HostName },
        { "RabbitMQ:UserName", config.Username },
        { "RabbitMQ:Password", config.Password },
        { "RabbitMQ:Port", config.Port.ToString() },
    };

    public Dictionary<string, string> GetPostgreSqlConfig(DatabaseConfiguration config)
        => new() { { "Database:PostgresEfCoreConnectionString", config.PostgresEfCoreConnectionString }, };

    public Dictionary<string, string> GetKickConfig(string connectionString)
        => new() { { "Kick:PusherConnectionString", connectionString }, };
}