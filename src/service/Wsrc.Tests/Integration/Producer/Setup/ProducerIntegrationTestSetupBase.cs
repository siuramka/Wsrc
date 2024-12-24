using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

using Wsrc.Tests.Integration.Fakes;

namespace Wsrc.Tests.Integration.Producer.Setup;

[TestFixture]
public abstract class ProducerIntegrationTestSetupBase
{
    protected RabbitMqContainer RabbitMqContainer = null!;
    protected PostgreSqlContainer PostgreSqlContainer = null!;
    protected FakePusherServer FakePusherServer = null!;

    private async Task SetupRabbitMqAsync()
    {
        RabbitMqContainer = new RabbitMqBuilder()
            .WithName($"rabbitmq-test-{Guid.NewGuid()}")
            .WithPortBinding(56720, true)
            .WithUsername("integration")
            .WithPassword("integration")
            .Build();

        await RabbitMqContainer.StartAsync();
    }

    protected Dictionary<string, string> RabbitMqConfig
        => new()
        {
            { "RabbitMQ:HostName", RabbitMqContainer.Hostname },
            { "RabbitMQ:Port", RabbitMqContainer.GetMappedPublicPort(56720).ToString() },
            { "RabbitMQ:UserName", "integration" },
            { "RabbitMQ:Password", "integration" },
            { "RabbitMQ:QueueName", "Wsrc" },
        };

    private async Task SetupPostgreSqlAsync()
    {
        PostgreSqlContainer = new PostgreSqlBuilder()
            .WithName($"postgresql-test-{Guid.NewGuid()}")
            .WithPortBinding(54320, true)
            .WithUsername("integration")
            .WithPassword("integration")
            .WithDatabase("WsrcDb")
            .Build();

        await PostgreSqlContainer.StartAsync();
    }

    protected Dictionary<string, string> PostgreSqlConfig
        => new() { { "Database:PostgresEfCoreConnectionString", PostgreSqlContainer.GetConnectionString() }, };

    protected static Dictionary<string, string> KickConfig => new()
    {
        { "Kick:PusherConnectionString", FakePusherServer.GetConnectionString() },
    };

    protected async Task SetupContainers()
    {
        await Task.WhenAll(
            SetupRabbitMqAsync(),
            SetupPostgreSqlAsync()
        );
    }

    protected async Task SetupFakes()
    {
        FakePusherServer = new FakePusherServer();
        await FakePusherServer.StartAsync();
    }

    [TearDown]
    public async Task CleanupFakes()
    {
        await FakePusherServer.DisposeAsync();
    }

    [TearDown]
    public async Task CleanupRabbitMq()
    {
        await RabbitMqContainer.StopAsync();
        await RabbitMqContainer.DisposeAsync();
    }

    [TearDown]
    public async Task CleanupPostgres()
    {
        await PostgreSqlContainer.StopAsync();
        await PostgreSqlContainer.DisposeAsync();
    }
}