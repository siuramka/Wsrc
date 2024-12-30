using Microsoft.Extensions.DependencyInjection;

using Wsrc.Core.Interfaces;
using Wsrc.Core.Interfaces.Mappings;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Core.Services.Kick;
using Wsrc.Core.Services.Kick.EventStrategies;
using Wsrc.Core.Services.Kick.EventStrategies.Consumer;
using Wsrc.Infrastructure.Interfaces;
using Wsrc.Infrastructure.Mappings;
using Wsrc.Infrastructure.Messaging;
using Wsrc.Infrastructure.Persistence.Efcore.Repositories;
using Wsrc.Infrastructure.Services;

namespace Wsrc.Infrastructure.Startup;

public static class ConsumerServiceRegister
{
    public static void RegisterConsumerServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));

        services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
        services.AddSingleton<IKickMessageSavingService, KickChatMessageBatchSavingService>();
        services.AddSingleton<IConsumerMessageProcessor, KickConsumerMessageProcessor>();
        services.AddSingleton<IConsumerService, RabbitMqConsumerService>();
        services.AddSingleton<IConsumerServiceAcknowledger, ConsumerServiceAcknowledger>();

        services.AddTransient<IKickEventStrategyHandler, KickEventStrategyHandler>();
        services.AddTransient<IKickEventStrategy, ChatMessageEvent>();

        services.AddSingleton<IKickChatMessageMapper, KickChatMessageMapper>();
        services.AddSingleton<IMapper, Mapper>();
    }
}