using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wsrc.Core.Interfaces;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Core.Services.Kick;
using Wsrc.Core.Services.Kick.EventStrategies;
using Wsrc.Core.Services.Kick.EventStrategies.Producer;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;
using Wsrc.Infrastructure.Messaging;
using Wsrc.Infrastructure.Persistence;
using Wsrc.Infrastructure.Persistence.Efcore.Repositories;
using Wsrc.Infrastructure.Services;
using Wsrc.Infrastructure.Startup;
using Wsrc.Producer.Services;

namespace Wsrc.Producer;

public class Program
{
    // add cancellation tokens
    // handle websocket drop connection
    // handle pusher drop/unsubscribe/etc connection
    // add tests
    // add prometheus metrics
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var configuration = builder.Configuration;

        builder.Configuration.AddEnvironmentVariables();
        builder.Services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.Section));
        builder.Services.Configure<KickConfiguration>(configuration.GetSection(KickConfiguration.Section));
        builder.Services.Configure<DatabaseConfiguration>(configuration.GetSection(DatabaseConfiguration.Section));

        builder.Services.AddDbContext<WsrcContext>((serviceProvider, options) =>
        {
            var dbConfig = serviceProvider.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
            options.UseNpgsql(dbConfig.PostgresEfCoreConnectionString);
        });

        builder.Services.RegisterProducerServices();

        builder.Services.AddHostedService<ProducerWorkerService>();

        var host = builder.Build();
        host.Run();
    }
}
