using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using Wsrc.Core.Interfaces;
using Wsrc.Producer.Services;
using Wsrc.Tests.Reusables.Mocks;

namespace Wsrc.Tests.Unit.Producer;

[TestFixture]
public class ProducerWorkerServiceTests
{
    private IKickProducerFacade _kickProducerFacade;
    private IPeriodicTimer _periodicTimer;

    private IServiceProvider _serviceProvider;
    private IServiceScopeFactory _serviceScopeFactory;

    private ProducerWorkerService _worker;

    [SetUp]
    public void SetUp()
    {
        _kickProducerFacade = Substitute.For<IKickProducerFacade>();
        _periodicTimer = Substitute.For<IPeriodicTimer>();

        (_serviceProvider, _serviceScopeFactory) = new ServiceProviderMock().SetupMock();
        _serviceProvider.GetService<IPeriodicTimer>().Returns(_periodicTimer);

        _worker = new ProducerWorkerService(_kickProducerFacade, _serviceScopeFactory);
    }

    [TearDown]
    public void TearDown()
    {
        _worker.Dispose();
    }

    [Test]
    public async Task WorkerService_ReconnectsClients_TimerElapsed()
    {
        // Arrange
        _periodicTimer
            .WaitForNextTickAsync(Arg.Any<CancellationToken>())
            .Returns(true, false);

        // Act
        await _worker.StartAsync(CancellationToken.None);

        // Assert
        await _kickProducerFacade.Received(1).HandleReconnectAsync();
    }

    [Test]
    public async Task WorkerService_DoesntReconnectsClients_TimerNotElapsed()
    {
        // Arrange
        _periodicTimer
            .WaitForNextTickAsync(Arg.Any<CancellationToken>())
            .Returns(false, false);

        // Act
        await _worker.StartAsync(CancellationToken.None);

        // Assert
        await _kickProducerFacade.DidNotReceive().HandleReconnectAsync();
    }
}