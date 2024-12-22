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
    public async Task HandleMessages_LaunchesClientManager()
    {
        // Act
        await _kickProducerFacade.HandleMessages();

        // Assert
        await _kickPusherClientManager.Received(1).Launch();
    }

    [Test]
    [TestCase(10)]
    [TestCase(100)]
    public async Task HandleMessages_ProcessesOnDifferentThreads(int clientCount)
    {
        // Arrange
        var clients = Enumerable
            .Range(0, clientCount)
            .Select(_ => Substitute.For<IKickPusherClient>())
            .ToList();

        _kickPusherClientManager.ActiveConnections.Returns(clients);

        var mainThreadId = Environment.CurrentManagedThreadId;

        var processingThreadIds = new ConcurrentBag<int>();
        var processingStarted = new TaskCompletionSource();

        _kickMessageProducerProcessor
            .ProcessChannelMessagesAsync(Arg.Any<IKickPusherClient>())
            .Returns(Task.CompletedTask)
            .AndDoes(x =>
            {
                processingThreadIds.Add(Environment.CurrentManagedThreadId);

                var isLastClient = processingThreadIds.Count == clientCount;
                if (isLastClient)
                {
                    processingStarted.SetResult();
                }
            });

        // Act
        await _kickProducerFacade.HandleMessages();

        await Task.WhenAny(processingStarted.Task);

        // Assert
        using var scope = new AssertionScope();

        processingThreadIds.Count.Should().Be(clientCount);
        processingThreadIds.Should().NotContain(mainThreadId);
    }
}
