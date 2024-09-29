using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;
using Wsrc.Infrastructure.Services;
using Wsrc.Infrastructure.Services.Kick;
using Wsrc.Producer.Services;

namespace Wsrc.Producer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var configuration = builder.Configuration;

        builder.Services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.Section));
        builder.Services.Configure<KickConfiguration>(configuration.GetSection(KickConfiguration.Section));

        builder.Services.AddSingleton<IKickPusherClientFactory, KickPusherClientFactory>();
        builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

        builder.Services.AddHostedService<ProducerWorkerService>();

        var host = builder.Build();
        host.Run();
    }
}
