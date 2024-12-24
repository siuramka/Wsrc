using Wsrc.Tests.Integration.Producer.Setup;

namespace Wsrc.Tests.Integration.Producer;

[TestFixture]
public class ProducerIntegrationTests : ProducerIntegrationTestBase
{
    [SetUp]
    public async Task SetUpAsync()
    {
        await InitializeAsync();
    }

    [Test]
    public async Task Producer_ConnectsChannelsAndSubscribesToPusher()
    {
        // Arrange
        const int channelsCount = 2;

        // Assert
        Assert.That(() => FakePusherServer.ActiveConnections.Count,
            Is.EqualTo(channelsCount).After(3).Seconds.PollEvery(250).MilliSeconds);
    }
}