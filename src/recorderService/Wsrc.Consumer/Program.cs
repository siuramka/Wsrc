using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;
using Wsrc.Infrastructure.Services;

namespace Wsrc.Consumer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var configuration = builder.Configuration;
        builder.Services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.Section));
        builder.Services.Configure<KickConfiguration>(configuration.GetSection(KickConfiguration.Section));

        builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
        builder.Services.AddSingleton<IConsumerService, KickConsumerService>();

        builder.Services.AddHostedService<ConsumerWorkerService>();

        var host = builder.Build();
        host.Run();
    }
}