using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Wsrc.Core.Services.Kick;

namespace Wsrc.Tests.Unit.Core.Services.Kick;

[TestFixture]
public class KickConsumerMessageProcessorTests
{
    private IServiceScopeFactory _serviceScopeFactory;

    private KickConsumerMessageProcessor _kickConsumerMessageProcessor;

    [SetUp]
    public void SetUp()
    {
        _serviceScopeFactory = Substitute.For<IServiceScopeFactory>();

        _kickConsumerMessageProcessor = new KickConsumerMessageProcessor(_serviceScopeFactory);
    }

    [Test]
    public async Task ConsumeAsync_ThrowsException_WhenKickEventDeserializeFailed()
    {
        // Arrange
        const string invalidData = "invalid data";

        // Act
        var consumeAsyncDelegate = async () =>
            await _kickConsumerMessageProcessor
                .ConsumeAsync(invalidData);

        // Assert
        await consumeAsyncDelegate.Should().ThrowExactlyAsync<JsonException>("Failed to deserialize message");
    }
}
