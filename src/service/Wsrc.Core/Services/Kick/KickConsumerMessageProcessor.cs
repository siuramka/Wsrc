using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Wsrc.Core.Interfaces;
using Wsrc.Domain;
using Wsrc.Domain.Models;

namespace Wsrc.Core.Services.Kick;

public class KickConsumerMessageProcessor(
    IServiceScopeFactory serviceScopeFactory
)
    : IConsumerMessageProcessor
{
    public async Task ConsumeAsync(string data)
    {
        var kickEvent = JsonSerializer.Deserialize<KickEvent>(data)
                        ?? throw new NotSupportedException("Failed to deserialize message");

        var pusherEvent = PusherEvent.Parse(kickEvent.Event);

        using var scope = serviceScopeFactory.CreateScope();
        var eventHandler = scope.ServiceProvider.GetRequiredService<IKickEventStrategyHandler>();

        var strategy = eventHandler.GetStrategy(pusherEvent);

        await strategy.ExecuteAsync(data);
    }
}
