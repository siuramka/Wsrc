using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Wsrc.Infrastructure.Persistence;
using Wsrc.Tests.Integration.Setup;
using Wsrc.Tests.Reusables.Helpers;
using Wsrc.Tests.Reusables.Providers;

namespace Wsrc.Tests.Integration;

public class ProducerConsumerTests : ProducerConsumerIntegrationTestBase
{
    [SetUp]
    public async Task SetUp()
    {
        await InitializeAsync();
    }

    [Test]
    [TestCase(100)]
    [TestCase(1000)]
    public async Task ProducerConsumer_ProducerWritesChatMessageToQueueAndConsumerBatchInsertsToDatabase(
        int messageCount)
    {
        // Arrange
        var firstChannelConnection = _fakePusherServer.ActiveConnections.First();
        var firstChannel = new ChannelProvider().ProvideDefault().First();
        var message = new kickChatMessageBufferProvider().ProvideSerialized(firstChannel.Id.ToString());

        // Act
        var messagePublishTasks = Enumerable.Range(0, messageCount)
            .Select(_ => _fakePusherServer.SendMessageAsync(firstChannelConnection, message));

        await Task.WhenAll(messagePublishTasks);

        // Assert
        using var scope = _consumerHost.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<WsrcContext>();

        var getHasRequiredMessageCountAsync = async ()
            => await context.Messages.CountAsync() == messageCount;

        await TimeoutHelper.WaitUntilAsync(getHasRequiredMessageCountAsync);
    }
}