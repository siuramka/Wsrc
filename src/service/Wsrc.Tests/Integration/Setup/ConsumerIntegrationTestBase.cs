using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Wsrc.Consumer;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Persistence;
using Wsrc.Infrastructure.Startup;
using Wsrc.Tests.Integration.Reusables.Helpers;

using RabbitMqConfiguration = Wsrc.Infrastructure.Configuration.RabbitMqConfiguration;

namespace Wsrc.Tests.Integration.Setup;

public abstract class ConsumerIntegrationTestBase : IntegrationTestBase
{
    protected IHost _host = null!;
    private readonly TestConfigurationsSetup _configSetup = new();

    protected async Task InitializeAsync()
    {
        await SetupContainersAsync();

        BuildHost();

        await MigrateDatabaseAsync(_host);
        await SeedRequiredDataAsync(_host);

        await _host.StartAsync();
    }

    [TearDown]
    public async Task CleanupAsync()
    {
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
}