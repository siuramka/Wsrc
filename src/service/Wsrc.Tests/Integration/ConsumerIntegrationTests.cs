using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Wsrc.Infrastructure.Persistence;
using Wsrc.Tests.Integration.Reusables.Stubs;
using Wsrc.Tests.Integration.Setup;
using Wsrc.Tests.Reusables.Helpers;
using Wsrc.Tests.Reusables.Providers;

namespace Wsrc.Tests.Integration;

[TestFixture]
public class ConsumerIntegrationTests : ConsumerIntegrationTestBase
{
    [SetUp]
    public async Task SetUp()
    {
        await InitializeAsync();
    }

    [Test]
    [TestCase(100)]
    [TestCase(1000)]
    public async Task Consumer_WritesKickChatMessagesToDatabase_WhenReceivedBatchMessageCount(int messageCount)
    {
        // Arrange
        var testRabbitMqClientStub = new TestRabbitMqClientStub();

        await testRabbitMqClientStub.ConnectAsync(RabbitMqConfiguration);

        var firstChannel = new ChannelProvider().ProvideDefault().First();

        var getKickChatMessageBufferString = () => new kickChatMessageBufferProvider()
            .ProvideSerialized(firstChannel.Id.ToString());

        // Act
        var messagePublishTasks = Enumerable.Range(0, messageCount)
            .Select(_ => testRabbitMqClientStub.PublishMessageAsync(getKickChatMessageBufferString()));

        await Task.WhenAll(messagePublishTasks);

        // Assert
        using var scope = _host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<WsrcContext>();

        var getHasRequiredMessageCountAsync = async ()
            => await context.Messages.CountAsync() == messageCount;

        await TimeoutHelper.WaitUntilAsync(getHasRequiredMessageCountAsync);
    }
}