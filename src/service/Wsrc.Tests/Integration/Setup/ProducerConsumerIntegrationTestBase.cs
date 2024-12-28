using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Wsrc.Consumer;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Persistence;
using Wsrc.Infrastructure.Startup;
using Wsrc.Producer.Services;
using Wsrc.Tests.Integration.Reusables.Fakes;
using Wsrc.Tests.Reusables.Helpers;
using Wsrc.Tests.Reusables.Providers;

namespace Wsrc.Tests.Integration.Setup;

public abstract class ProducerConsumerIntegrationTestBase : IntegrationTestBase
{
    protected FakePusherServer _fakePusherServer = null!;
    protected IHost _producerHost = null!;
    protected IHost _consumerHost = null!;

    protected async Task InitializeAsync()
    {
        await Task.WhenAll(
            SetupContainersAsync(),
            SetupFakes()
        );

        BuildProducerHost();
        BuildConsumerHost();

        await UpdateDatabase();
        await SeedRequiredData();

        await Task.WhenAll(
            _producerHost.StartAsync(),
            _consumerHost.StartAsync()
        );

        await WaitForPusherSubscriptions();
    }

    [TearDown]
    public async Task Cleanup()
    {
        await Task.WhenAll(
            _producerHost.StopAsync(),
            _consumerHost.StopAsync(),
            _fakePusherServer.StopAsync()
        );

        _producerHost.Dispose();
        _consumerHost.Dispose();
        await _fakePusherServer.DisposeAsync();

        await CleanupContainers();
    }

    private void BuildProducerHost()
    {
        _producerHost = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection(RabbitMqConfig!);
                config.AddInMemoryCollection(PostgreSqlConfig!);
                config.AddInMemoryCollection(KickConfig!);
            })
            .ConfigureServices((context, services) =>
            {
                services.RegisterProducerServices();

                var configuration = context.Configuration;

                services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.Section));
                services.Configure<KickConfiguration>(configuration.GetSection(KickConfiguration.Section));
                services.Configure<DatabaseConfiguration>(configuration.GetSection(DatabaseConfiguration.Section));

                services.AddDbContext<WsrcContext>((_, options) =>
                {
                    options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
                });

                services.AddHostedService<ProducerWorkerService>();
            })
            .Build();
    }

    private void BuildConsumerHost()
    {
        _consumerHost = Host.CreateDefaultBuilder()
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

    private static Dictionary<string, string> KickConfig => new()
    {
        { "Kick:PusherConnectionString", FakePusherServer.GetConnectionString() },
    };

    private async Task SetupFakes()
    {
        _fakePusherServer = new FakePusherServer();
        await _fakePusherServer.StartAsync();
    }

    private async Task WaitForPusherSubscriptions()
    {
        const int channelsCount = 2;

        var getHasActiveConnections = () => _fakePusherServer.ActiveConnections.Count == channelsCount;

        await TimeoutHelper.WaitUntilAsync(getHasActiveConnections);
    }

    private async Task UpdateDatabase()
    {
        using var scope = _producerHost.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<WsrcContext>();

        await context.Database.MigrateAsync();
    }

    private async Task SeedRequiredData()
    {
        using var scope = _producerHost.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WsrcContext>();

        var channels = new ChannelProvider().ProvideDefault();
        var chatrooms = new ChatroomProvider().ProvideDefault();

        await context.Channels.AddRangeAsync(channels);
        await context.Chatrooms.AddRangeAsync(chatrooms);
        await context.SaveChangesAsync();
    }
}