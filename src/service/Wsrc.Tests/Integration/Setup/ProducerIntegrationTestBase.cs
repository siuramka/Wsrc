using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Persistence;
using Wsrc.Infrastructure.Startup;
using Wsrc.Producer.Services;
using Wsrc.Tests.Integration.Reusables.Fakes;
using Wsrc.Tests.Reusables.Helpers;
using Wsrc.Tests.Reusables.Providers;

using RabbitMqConfiguration = Wsrc.Infrastructure.Configuration.RabbitMqConfiguration;

namespace Wsrc.Tests.Integration.Setup;

public abstract class ProducerIntegrationTestBase : IntegrationTestBase
{
    protected FakePusherServer _fakePusherServer = null!;
    private IHost _host = null!;

    protected async Task InitializeAsync()
    {
        await Task.WhenAll(
            SetupContainersAsync(),
            SetupFakes()
        );

        BuildHost();

        await UpdateDatabase();
        await SeedRequiredData();

        await _host.StartAsync();

        await WaitForPusherSubscriptions();
    }

    [TearDown]
    public async Task Cleanup()
    {
        await _fakePusherServer.StopAsync();
        await _fakePusherServer.DisposeAsync();

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

        await context.Channels.AddRangeAsync(channels);
        await context.SaveChangesAsync();
    }
}