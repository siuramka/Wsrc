using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Wsrc.Consumer;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Persistence;
using Wsrc.Infrastructure.Startup;
using Wsrc.Tests.Reusables.Providers;

using RabbitMqConfiguration = Wsrc.Infrastructure.Configuration.RabbitMqConfiguration;

namespace Wsrc.Tests.Integration.Setup;

public abstract class ConsumerIntegrationTestBase : IntegrationTestBase
{
    protected IHost _host = null!;

    protected async Task InitializeAsync()
    {
        await SetupContainersAsync();

        BuildHost();

        await UpdateDatabase();
        await SeedRequiredData();

        await _host.StartAsync();
    }

    [TearDown]
    public async Task Cleanup()
    {
        await _host.StopAsync();
        _host.Dispose();

        await CleanupContainers();
    }

    private void BuildHost()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection(RabbitMqConfig!);
                config.AddInMemoryCollection(PostgreSqlConfig!);
            })
            .ConfigureServices((context, services) =>
            {
                services.RegisterConsumerServices();

                var configuration = context.Configuration;

                services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.Section));
                services.Configure<DatabaseConfiguration>(configuration.GetSection(DatabaseConfiguration.Section));

                services.AddDbContext<WsrcContext>((_, options) =>
                {
                    options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
                });

                services.AddHostedService<ConsumerWorkerService>();
            })
            .Build();
    }

    private Dictionary<string, string> PostgreSqlConfig
        => new()
        {
            { "Database:PostgresEfCoreConnectionString", DatabaseConfiguration.PostgresEfCoreConnectionString },
        };

    private Dictionary<string, string> RabbitMqConfig
        => new()
        {
            { "RabbitMQ:HostName", RabbitMqConfiguration.HostName },
            { "RabbitMQ:UserName", RabbitMqConfiguration.Username },
            { "RabbitMQ:Password", RabbitMqConfiguration.Password },
            { "RabbitMQ:Port", RabbitMqConfiguration.Port.ToString() },
        };

    private async Task UpdateDatabase()
    {
        using var scope = _host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<WsrcContext>();

        await context.Database.MigrateAsync();
    }

    private async Task SeedRequiredData()
    {
        using var scope = _host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WsrcContext>();

        var channels = new ChannelProvider().ProvideDefault();
        var chatrooms = new ChatroomProvider().ProvideDefault();

        await context.Channels.AddRangeAsync(channels);
        await context.Chatrooms.AddRangeAsync(chatrooms);

        await context.SaveChangesAsync();
    }
}