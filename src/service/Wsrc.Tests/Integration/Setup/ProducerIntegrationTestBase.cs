using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Persistence;
using Wsrc.Infrastructure.Startup;
using Wsrc.Producer.Services;
using Wsrc.Tests.Integration.Reusables.Fakes;
using Wsrc.Tests.Integration.Reusables.Helpers;
using Wsrc.Tests.Reusables.Helpers;

using RabbitMqConfiguration = Wsrc.Infrastructure.Configuration.RabbitMqConfiguration;

namespace Wsrc.Tests.Integration.Setup;

public abstract class ProducerIntegrationTestBase : IntegrationTestBase
{
    protected FakePusherServer _fakePusherServer = null!;
    private IHost _host = null!;

    private readonly TestConfigurationsSetup _configSetup = new();

    protected async Task InitializeAsync()
    {
        await Task.WhenAll(
            SetupContainersAsync(),
            SetupFakesAsync()
        );

        BuildHost();

        await UpdateDatabaseAsync(_host);
        await SeedRequiredDataAsync(_host);

        await _host.StartAsync();

        await WaitForPusherSubscriptionsAsync();
    }

    [TearDown]
    public async Task CleanupAsync()
    {
        await _fakePusherServer.StopAsync();
        await _fakePusherServer.DisposeAsync();

        await _host.StopAsync();
        _host.Dispose();

        await CleanupContainersAsync();
    }

    private void BuildHost()
    {
        _host = Host.CreateDefaultBuilder()
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