using System.Text;
using System.Text.Json;

using FluentAssertions;

using Wsrc.Tests.Integration.Reusables.Stubs;
using Wsrc.Tests.Integration.Setup;
using Wsrc.Tests.Reusables.Providers;

namespace Wsrc.Tests.Integration;

[TestFixture]
public class ProducerIntegrationTests : ProducerIntegrationTestBase
{
    [Test]
    public void Producer_ConnectsChannelsAndSubscribesToPusher()
    {
        // Assert
        const int channelsCount = 2;
        _fakePusherServer.ActiveConnections.Count.Should().Be(channelsCount);
    }

    [Test]
    public async Task Producer_WhenPusherChatMessageEventReceived_ShouldWriteSerializedMessageToRabbitMqQueue()
    {
        // Arrange
        var testRabbitMqClientStub = new TestRabbitMqClientStub();

        await testRabbitMqClientStub.ConnectAsync(RabbitMqConfiguration);

        var receivedMessageTask = new TaskCompletionSource<string>();

        await testRabbitMqClientStub.ConsumeMessagesAsync((_, ea) =>
        {
            var body = ea.Body.ToArray();
            var consumedMessage = Encoding.UTF8.GetString(body);

            receivedMessageTask.SetResult(consumedMessage);

            return Task.CompletedTask;
        });

        var firstChannelConnection = _fakePusherServer
            .ActiveConnections
            .First()
            .Key;

        var message = new KickChatMessageProvider().Create();

        // Act
        await _fakePusherServer.SendMessageAsync(firstChannelConnection, message);

        // Assert
        var consumedMessage = await receivedMessageTask.Task;
        var shouldBeMessage = JsonSerializer.Serialize(message);
        consumedMessage.Should().Be(shouldBeMessage);
    }
}