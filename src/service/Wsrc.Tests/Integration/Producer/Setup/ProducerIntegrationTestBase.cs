using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wsrc.Domain.Entities;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Persistence;
using Wsrc.Infrastructure.Startup;
using Wsrc.Producer.Services;
using RabbitMqConfiguration = Wsrc.Infrastructure.Configuration.RabbitMqConfiguration;

namespace Wsrc.Tests.Integration.Producer.Setup;

[TestFixture]
public abstract class ProducerIntegrationTestBase : ProducerIntegrationTestSetupBase
{
    protected IHost _host = null!;

    protected async Task InitializeAsync()
    {
        await Task.WhenAll(
            SetupContainers(),
            SetupFakes()
        );

        _host = Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(config =>
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
                    options.UseNpgsql(PostgreSqlContainer.GetConnectionString());
                });

                services.AddHostedService<ProducerWorkerService>();
            })
            .Build();

        await UpdateDatabase();
        await SeedRequiredData();

        await _host.StartAsync();
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

        if (context.Channels.Any())
        {
            return;
        }

        List<Channel> channels =
        [
            new()
            {
                Id = 11111,
                Name = "TestChannel1",
            },
            new()
            {
                Id = 22222,
                Name = "TestChannel2",
            }
        ];

        await context.Channels.AddRangeAsync(channels);
        await context.SaveChangesAsync();
    }

    [TearDown]
    public async Task CleanupHostedService()
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}
