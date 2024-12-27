using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

using Wsrc.Infrastructure.Configuration;

using RabbitMqConfiguration = Wsrc.Infrastructure.Configuration.RabbitMqConfiguration;

namespace Wsrc.Tests.Integration.Setup;

public abstract class IntegrationTestBase
{
    protected RabbitMqContainer _rabbitMqContainer = null!;
    protected PostgreSqlContainer _postgreSqlContainer = null!;

    protected CancellationTokenSource _timeoutToken = new(DefaultTimeout);
    protected readonly TimeSpan _pollInterval = TimeSpan.FromMilliseconds(250);
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    protected void ResetTimeoutToken()
    {
        _timeoutToken = new CancellationTokenSource(DefaultTimeout);
    }

    protected RabbitMqConfiguration RabbitMqConfiguration => new()
    {
        HostName = _rabbitMqContainer.Hostname,
        Username = "integration",
        Password = "integration",
        Port = _rabbitMqContainer.GetMappedPublicPort(5672),
    };

    protected DatabaseConfiguration DatabaseConfiguration => new()
    {
        PostgresEfCoreConnectionString = $"{_postgreSqlContainer.GetConnectionString()};IncludeErrorDetail=True",
    };

    protected async Task SetupContainersAsync()
    {
        await Task.WhenAll(
            SetupRabbitMqAsync(),
            SetupPostgreSqlAsync()
        );
    }

    protected async Task CleanupContainers()
    {
        await Task.WhenAll(
            CleanupRabbitMq(),
            CleanupPostgres()
        );
    }

    private async Task SetupPostgreSqlAsync()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithName($"postgresql-integration-{Guid.NewGuid()}")
            .WithPortBinding(54320, true)
            .WithUsername("integration")
            .WithPassword("integration")
            .WithDatabase("integration")
            .Build();

        await _postgreSqlContainer.StartAsync();
    }

    private async Task SetupRabbitMqAsync()
    {
        _rabbitMqContainer = new RabbitMqBuilder()
            .WithName($"rabbitmq-integration-{Guid.NewGuid()}")
            .WithPortBinding(5672, true)
            .WithUsername("integration")
            .WithPassword("integration")
            .Build();

        await _rabbitMqContainer.StartAsync();
    }

    private async Task CleanupRabbitMq()
    {
        await _rabbitMqContainer.StopAsync();
        await _rabbitMqContainer.DisposeAsync();
    }

    private async Task CleanupPostgres()
    {
        await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
    }
}