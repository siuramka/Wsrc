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
using Wsrc.Tests.Integration.Reusables.Helpers;
using Wsrc.Tests.Reusables.Helpers;

namespace Wsrc.Tests.Integration.Setup;

public abstract class ProducerConsumerIntegrationTestBase : IntegrationTestBase
{
    protected FakePusherServer _fakePusherServer = null!;
    protected IHost _producerHost = null!;
    protected IHost _consumerHost = null!;

    private readonly TestConfigurationsSetup _configSetup = new();

    protected async Task InitializeAsync()
    {
        await Task.WhenAll(
            SetupContainersAsync(),
            SetupFakesAsync()
        );

        BuildProducerHost();
        BuildConsumerHost();

        await UpdateDatabaseAsync(_producerHost);
        await SeedRequiredDataAsync(_producerHost);

        await Task.WhenAll(
            _producerHost.StartAsync(),
            _consumerHost.StartAsync()
        );

        await WaitForPusherSubscriptionsAsync();
    }

    [TearDown]
    public async Task CleanupAsync()
    {
        await Task.WhenAll(
            _producerHost.StopAsync(),
            _consumerHost.StopAsync(),
            _fakePusherServer.StopAsync()
        );

        _producerHost.Dispose();
        _consumerHost.Dispose();
        await _fakePusherServer.DisposeAsync();

        await CleanupContainersAsync();
    }

    private void BuildProducerHost()
    {
        _producerHost = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection(_configSetup.GetRabbitMqConfig(RabbitMqConfiguration)!);
                config.AddInMemoryCollection(_configSetup.GetPostgreSqlConfig(DatabaseConfiguration)!);
                config.AddInMemoryCollection(_configSetup.GetKickConfig(FakePusherServer.GetConnectionString())!);
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
                config.AddInMemoryCollection(_configSetup.GetRabbitMqConfig(RabbitMqConfiguration)!);
                config.AddInMemoryCollection(_configSetup.GetPostgreSqlConfig(DatabaseConfiguration)!);
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

    private static Dictionary<string, string> KickConfig => new()
    {
        { "Kick:PusherConnectionString", FakePusherServer.GetConnectionString() },
    };

    private async Task SetupFakesAsync()
    {
        _fakePusherServer = new FakePusherServer();
        await _fakePusherServer.StartAsync();
    }

    private async Task WaitForPusherSubscriptionsAsync()
    {
        const int channelsCount = 2;

        var getHasActiveConnections = () => _fakePusherServer.ActiveConnections.Count == channelsCount;

        await TimeoutHelper.WaitUntilAsync(getHasActiveConnections);
    }
}