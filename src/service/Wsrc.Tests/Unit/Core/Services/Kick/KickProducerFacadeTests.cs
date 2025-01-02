using System.Collections.Concurrent;

using FluentAssertions;
using FluentAssertions.Execution;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using Wsrc.Core.Interfaces;
using Wsrc.Core.Services.Kick;
using Wsrc.Tests.Reusables.Mocks;

namespace Wsrc.Tests.Unit.Core.Services.Kick;

[TestFixture]
public class KickProducerFacadeTests
{
    private IServiceProvider _serviceProvider;
    private IServiceScopeFactory _serviceScopeFactory;

    private IKickPusherClientManager _kickPusherClientManager;
    private IKickMessageProducerProcessor _kickMessageProducerProcessor;

    private KickProducerFacade _kickProducerFacade;

    [SetUp]
    public void SetUp()
    {
        (_serviceProvider, _serviceScopeFactory) = new ServiceProviderMock().SetupMock();

        _kickPusherClientManager = Substitute.For<IKickPusherClientManager>();
        _kickMessageProducerProcessor = Substitute.For<IKickMessageProducerProcessor>();

        _serviceProvider.GetService<IKickMessageProducerProcessor>().Returns(_kickMessageProducerProcessor);

        _kickProducerFacade = new KickProducerFacade(_kickPusherClientManager, _serviceScopeFactory);
    }

    [Test]
    public async Task InitializeAsync_LaunchesClientManager()
    {
        // Arrange
        _kickPusherClientManager.GetActiveClients().Returns([]);

        // Act
        await _kickProducerFacade.InitializeAsync();

        // Assert
        await _kickPusherClientManager.Received(1).LaunchAsync();
    }

    [Test]
    [TestCase(10)]
    [TestCase(1000)]
    public async Task InitializeAsync_ProcessesOnDifferentThreads(int clientCount)
    {
        // Arrange
        var clients = Enumerable
            .Range(0, clientCount)
            .Select(_ => Substitute.For<IKickPusherClient>())
            .ToList();

        _kickPusherClientManager.GetActiveClients().Returns(clients);

        var mainThreadId = Environment.CurrentManagedThreadId;

        var processingThreadIds = new ConcurrentBag<int>();
        var processingStarted = new TaskCompletionSource();

        object threadLock = new();

        _kickMessageProducerProcessor
            .ProcessChannelMessagesAsync(Arg.Any<IKickPusherClient>())
            .Returns(Task.CompletedTask)
            .AndDoes(x =>
            {
                lock (threadLock)
                {
                    processingThreadIds.Add(Environment.CurrentManagedThreadId);

                    var isLastClient = processingThreadIds.Count == clientCount;
                    if (isLastClient)
                    {
                        processingStarted.SetResult();
                    }
                }
            });

        // Act
        await _kickProducerFacade.InitializeAsync();

        await Task.WhenAny(processingStarted.Task);

        // Assert
        using var scope = new AssertionScope();

        processingThreadIds.Count.Should().Be(clientCount);
        processingThreadIds.Should().NotContain(mainThreadId);
    }

    [Test]
    [TestCase(10)]
    [TestCase(1000)]
    public async Task InitializeAsync_StartsProcessingClients(int clientCount)
    {
        // Arrange
        var initialClients = Enumerable
            .Range(0, clientCount)
            .Select(_ => Substitute.For<IKickPusherClient>())
            .ToList();

        _kickPusherClientManager.GetActiveClients().Returns(initialClients);

        // Act
        await _kickProducerFacade.InitializeAsync();

        // Assert
        await _kickMessageProducerProcessor
            .ProcessChannelMessagesAsync(Arg.Is<IKickPusherClient>(client => initialClients.Contains(client)));
    }

    [Test]
    [TestCase(10)]
    [TestCase(1000)]
    public async Task HandleReconnectAsync_StartsProcessingClients(int clientCount)
    {
        // Arrange
        var initialClients = Enumerable
            .Range(0, clientCount)
            .Select(_ => Substitute.For<IKickPusherClient>())
            .ToList();

        _kickPusherClientManager.GetActiveClients().Returns(initialClients);

        // Act
        await _kickProducerFacade.HandleReconnectAsync();

        // Assert
        await _kickMessageProducerProcessor
            .ProcessChannelMessagesAsync(Arg.Is<IKickPusherClient>(client => initialClients.Contains(client)));
    }

    [Test]
    public async Task HandleReconnectAsync_StartsProcessingClients_WhenAlreadyHaveActiveClients()
    {
        // Arrange
        var initialClients = Enumerable
            .Range(0, 5)
            .Select(_ => Substitute.For<IKickPusherClient>())
            .ToList();

        _kickPusherClientManager.GetActiveClients().Returns(initialClients);

        await _kickProducerFacade.InitializeAsync();

        var newClients = Enumerable
            .Range(0, 5)
            .Select(_ => Substitute.For<IKickPusherClient>())
            .ToList();

        _kickPusherClientManager.GetActiveClients().Returns(newClients);

        // Act
        await _kickProducerFacade.HandleReconnectAsync();

        // Assert
        var allClients = initialClients.Concat(newClients).ToList();
        await _kickMessageProducerProcessor
            .ProcessChannelMessagesAsync(Arg.Is<IKickPusherClient>(client => allClients.Contains(client)));

    }
}