using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Persistence;
using Wsrc.Infrastructure.Startup;

namespace Wsrc.Consumer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var configuration = builder.Configuration;

        builder.Configuration.AddEnvironmentVariables();
        builder.Services.Configure<DatabaseConfiguration>(configuration.GetSection(DatabaseConfiguration.Section));
        builder.Services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.Section));

        builder.Services.AddDbContext<WsrcContext>((serviceProvider, options) =>
        {
            var dbConfig = serviceProvider.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
            options.UseNpgsql(dbConfig.PostgresEfCoreConnectionString);
        });

        builder.Services.RegisterConsumerServices();

        builder.Services.AddHostedService<ConsumerWorkerService>();

        var host = builder.Build();
        host.Run();
    }
}