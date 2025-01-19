using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Persistence;
using Wsrc.Tests.Integration.Reusables.Constants;
using Wsrc.Tests.Integration.Reusables.Helpers;

using RabbitMqConfiguration = Wsrc.Infrastructure.Configuration.RabbitMqConfiguration;

namespace Wsrc.Tests.Integration.Setup;

public abstract class IntegrationTestBase
{
    protected RabbitMqContainer _rabbitMqContainer = null!;
    protected PostgreSqlContainer _postgreSqlContainer = null!;

    protected RabbitMqConfiguration RabbitMqConfiguration => new()
    {
        HostName = _rabbitMqContainer.Hostname,
        Username = ContainerConstants.TestUsername,
        Password = ContainerConstants.TestPassword,
        Port = _rabbitMqContainer.GetMappedPublicPort(ContainerConstants.RabbitMqPort),
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

    protected async Task CleanupContainersAsync()
    {
        await Task.WhenAll(
            CleanupRabbitMqAsync(),
            CleanupPostgresAsync()
        );
    }
    
    protected static async Task ClearDatabaseAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WsrcContext>();

        const string query = 
            @"
                DELETE FROM ""Messages"";
                DELETE FROM ""Senders"";
                DELETE FROM ""Chatrooms"";
                DELETE FROM ""Channels"";
            ";
        
        await context.Database.ExecuteSqlRawAsync(query);
    }

    protected static async Task MigrateDatabaseAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<WsrcContext>();

        await context.Database.MigrateAsync();
    }

    protected static async Task SeedRequiredDataAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WsrcContext>();

        var seeder = new DefaultDataSeeder();
        await seeder.SeedRequiredDataAsync(context);
    }

    private async Task SetupPostgreSqlAsync()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithName($"postgresql-integration-{Guid.NewGuid()}")
            .WithPortBinding(ContainerConstants.PostgresPort, true)
            .WithUsername(ContainerConstants.TestUsername)
            .WithPassword(ContainerConstants.TestPassword)
            .WithDatabase(ContainerConstants.TestDatabase)
            .Build();

        await _postgreSqlContainer.StartAsync();
    }

    private async Task SetupRabbitMqAsync()
    {
        _rabbitMqContainer = new RabbitMqBuilder()
            .WithName($"rabbitmq-integration-{Guid.NewGuid()}")
            .WithPortBinding(ContainerConstants.RabbitMqPort, true)
            .WithUsername(ContainerConstants.TestUsername)
            .WithPassword(ContainerConstants.TestPassword)
            .Build();

        await _rabbitMqContainer.StartAsync();
    }

    private async Task CleanupRabbitMqAsync()
    {
        await _rabbitMqContainer.StopAsync();
        await _rabbitMqContainer.DisposeAsync();
    }

    private async Task CleanupPostgresAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
    }
}