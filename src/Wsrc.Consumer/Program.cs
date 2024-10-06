using Wsrc.Core.Interfaces;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;
using Wsrc.Infrastructure.Services;

namespace Wsrc.Consumer;

public class Program
{
    // todo
    // consume and parse messages
    // batch insert into db
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var configuration = builder.Configuration;

        builder.Services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.Section));
        builder.Services.Configure<KickConfiguration>(configuration.GetSection(KickConfiguration.Section));

        builder.Services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
        builder.Services.AddSingleton<IConsumerService, KickConsumerService>();

        builder.Services.AddHostedService<ConsumerWorkerService>();

        var host = builder.Build();
        host.Run();
    }
}
