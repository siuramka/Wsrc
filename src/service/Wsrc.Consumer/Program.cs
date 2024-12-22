using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wsrc.Core.Interfaces;
using Wsrc.Core.Interfaces.Mappings;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Core.Services.Kick;
using Wsrc.Core.Services.Kick.EventStrategies;
using Wsrc.Core.Services.Kick.EventStrategies.Consumer;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;
using Wsrc.Infrastructure.Mappings;
using Wsrc.Infrastructure.Messaging;
using Wsrc.Infrastructure.Persistence;
using Wsrc.Infrastructure.Persistence.Efcore.Repositories;
using Wsrc.Infrastructure.Services;

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
        builder.Services.Configure<KickConfiguration>(configuration.GetSection(KickConfiguration.Section));

        builder.Services.AddDbContext<WsrcContext>((serviceProvider, options) =>
        {
            var dbConfig = serviceProvider.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
            options.UseNpgsql(dbConfig.PostgresEfCoreConnectionString);
        });

        builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));

        builder.Services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
        builder.Services.AddSingleton<IKickMessageSavingService, KickChatMessageBatchSavingService>();
        builder.Services.AddSingleton<IConsumerMessageProcessor, KickConsumerMessageProcessor>();
        builder.Services.AddSingleton<IConsumerService, ConsumerService>();
        
        builder.Services.AddTransient<IKickEventStrategyHandler, KickEventStrategyHandler>();
        builder.Services.AddTransient<IKickEventStrategy, ChatMessageEvent>();
        
        builder.Services.AddSingleton<IKickChatMessageMapper, KickChatMessageMapper>();
        builder.Services.AddSingleton<IMapper, Mapper>();

        builder.Services.AddHostedService<ConsumerWorkerService>();

        var host = builder.Build();
        host.Run();
    }
}