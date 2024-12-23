using Microsoft.Extensions.DependencyInjection;
using Wsrc.Core.Interfaces;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Core.Services.Kick;
using Wsrc.Core.Services.Kick.EventStrategies;
using Wsrc.Core.Services.Kick.EventStrategies.Producer;
using Wsrc.Infrastructure.Interfaces;
using Wsrc.Infrastructure.Messaging;
using Wsrc.Infrastructure.Persistence.Efcore.Repositories;
using Wsrc.Infrastructure.Services;

namespace Wsrc.Infrastructure.Startup;

public static class ProducerServiceRegister
{
    public static void RegisterProducerServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));

        services.AddSingleton<IKickPusherClientFactory, KickPusherClientFactory>();
        services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
        services.AddSingleton<IProducerService, RabbitMqProducer>();
        services.AddSingleton<IKickProducerFacade, KickProducerFacade>();
        services.AddSingleton<IKickPusherClientManager, KickPusherClientManager>();

        services.AddTransient<IKickEventStrategy, ChatMessageEvent>();
        services.AddTransient<IKickEventStrategy, ConnectedEvent>();
        services.AddTransient<IKickEventStrategy, PongEvent>();
        services.AddTransient<IKickEventStrategy, SubscribedEvent>();
        services.AddTransient<IKickEventStrategyHandler, KickEventStrategyHandler>();

        services.AddScoped<IKickMessageProducerProcessor, KickProducerMessageProcessor>();
    }
}
