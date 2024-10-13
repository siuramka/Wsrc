using Wsrc.Core.Interfaces;
using Wsrc.Core.Services.Kick;
using Wsrc.Core.Services.Kick.EventStrategies;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;
using Wsrc.Infrastructure.Messaging;
using Wsrc.Infrastructure.Services;
using Wsrc.Producer.Services;

namespace Wsrc.Producer;

public class Program
{
    //TODO: add cancellation tokens
    // handle websocket drop connection
    // handle pusher drop/unsubscribe/etc connection
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var configuration = builder.Configuration;

        builder.Services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.Section));
        builder.Services.Configure<KickConfiguration>(configuration.GetSection(KickConfiguration.Section));

        builder.Services.AddSingleton<IKickPusherClientFactory, KickPusherClientFactory>();
        builder.Services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
        builder.Services.AddSingleton<IProducerService, RabbitMqProducer>();
        builder.Services.AddSingleton<IKickProducerFacede, KickProducerFacade>();
        builder.Services.AddSingleton<IKickPusherClientManager, KickPusherClientManager>();

        builder.Services.AddTransient<IKickEventStrategy, ChatMessageEvent>();
        builder.Services.AddTransient<IKickEventStrategy, ConnectedEvent>();
        builder.Services.AddTransient<IKickEventStrategy, PongEvent>();
        builder.Services.AddTransient<IKickEventStrategy, SubscribedEvent>();

        builder.Services.AddTransient<IKickEventStrategyHandler, KickEventStrategyHandler>();

        builder.Services.AddScoped<IKickMessageProducerProcessor, KickProducerMessageProcessor>();

        builder.Services.AddHostedService<ProducerWorkerService>();

        var host = builder.Build();
        host.Run();
    }
}
