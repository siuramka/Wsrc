using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;
using Wsrc.Infrastructure.Services;
using Wsrc.Infrastructure.Services.Kick;
using Wsrc.Infrastructure.Services.Kick.EventStrategies;
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

        //todo fix lifetimes
        builder.Services.AddSingleton<IKickPusherClientFactory, KickPusherClientFactory>();
        builder.Services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
        builder.Services.AddSingleton<IProducerService, RabbitMqProducer>();
        builder.Services.AddSingleton<IKickPusherClientManager, KickPusherClientManager>();
        builder.Services.AddSingleton<IKickEventStrategy, ChatMessageEvent>();
        builder.Services.AddSingleton<IKickEventStrategy, ConnectedEvent>();
        builder.Services.AddSingleton<IKickEventStrategy, PongEvent>();
        builder.Services.AddSingleton<IKickEventStrategy, SubscribedEvent>();
        builder.Services.AddSingleton<IKickEventStrategyHandler, KickEventStrategyHandler>();
        builder.Services.AddSingleton<IKickProducerFacede, KickProducerFacade>();

        builder.Services.AddHostedService<ProducerWorkerService>();

        var host = builder.Build();
        host.Run();
    }
}
