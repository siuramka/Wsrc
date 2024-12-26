using System.Text;
using System.Text.Json;

using FluentAssertions;

using RabbitMQ.Client;

using Wsrc.Tests.Integration.Reusables.Stubs;
using Wsrc.Tests.Integration.Setup;
using Wsrc.Tests.Reusables.Providers;

namespace Wsrc.Tests.Integration;

[TestFixture]
public class ProducerIntegrationTests : ProducerIntegrationTestBase
{
    //todo figure out when to dispose/setup etc
    [SetUp]
    public async Task SetUpAsync()
    {
        await InitializeAsync();
    }

    [Test]
    public void Producer_ConnectsChannelsAndSubscribesToPusher()
    {
        // Arrange
        const int channelsCount = 2;

        // Assert
        _fakePusherServer.ActiveConnections.Count.Should().Be(channelsCount);
    }

    [Test]
    public async Task Producer_WhenPusherChatMessageEventReceived_ShouldWriteSerializedMessageToRabbitMqQueue()
    {
        // Arrange
        var testRabbitMqClientStub = new TestRabbitMqClientStub();

        await testRabbitMqClientStub.ConnectAsync(
            new ConnectionFactory
            {
                UserName = RabbitMqConfiguration.Username,
                Port = RabbitMqConfiguration.Port,
                Password = RabbitMqConfiguration.Password,
            }
        );

        var receivedMessageTask = new TaskCompletionSource<string>();

        await testRabbitMqClientStub.ConsumeMessagesAsync(async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var consumedMessage = Encoding.UTF8.GetString(body);

            receivedMessageTask.SetResult(consumedMessage);
        });

        var firstChannelConnection = _fakePusherServer.ActiveConnections.First();
        var message = new KickChatMessageProvider().Create();

        // Act
        await _fakePusherServer.SendMessageAsync(firstChannelConnection, message);

        // Assert
        var shouldBeMessage = JsonSerializer.Serialize(message);
        var consumedMessage = await receivedMessageTask.Task;
        consumedMessage.Should().Be(shouldBeMessage);
    }
}