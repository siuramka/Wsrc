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
    public async Task ConsumeAsync(MessageEnvelope messageEnvelope)
    {
        var kickEvent = JsonSerializer.Deserialize<KickEvent>(messageEnvelope.Payload.ToString()!);

        var pusherEvent = PusherEvent.Parse(kickEvent!.Event);

        using var scope = serviceScopeFactory.CreateScope();
        var eventHandler = scope.ServiceProvider.GetRequiredService<IKickEventStrategyHandler>();

        var strategy = eventHandler.GetStrategy(pusherEvent);

        await strategy.ExecuteAsync(messageEnvelope);
    }
}