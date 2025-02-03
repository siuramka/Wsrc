using FluentAssertions;
using FluentAssertions.Execution;

using NSubstitute;

using Wsrc.Core.Interfaces;
using Wsrc.Infrastructure.Services;

namespace Wsrc.Tests.Unit.Infrastructure.Services;

[TestFixture]
public class ActiveClientsManagerTests
{
    private ActiveClientsManager _activeClientsManager;

    [SetUp]
    public void SetUp()
    {
        _activeClientsManager = new ActiveClientsManager();
    }

    [Test]
    public async Task AddAsync_AddsClientAsync()
    {
        // Arrange
        var client = Substitute.For<IKickPusherClient>();

        // Act
        await _activeClientsManager.AddAsync(client);

        // Assert
        const int activeClientCount = 1;

        using AssertionScope scope = new();
        _activeClientsManager.GetActiveClients().Count().Should().Be(activeClientCount);
        _activeClientsManager.GetActiveClients().Should().BeEquivalentTo([client]);
    }

    [Test]
    [TestCase(1000)]
    [TestCase(20000)]
    public async Task AddAsync_AddsClientsConcurrentlyAsync(int activeClientCount)
    {
        // Arrange
        var clients = Enumerable.Range(0, activeClientCount)
            .Select(_ => Substitute.For<IKickPusherClient>())
            .ToList();

        // Act
        var addTasks = clients.Select(c => _activeClientsManager.AddAsync(c));

        await Task.WhenAll(addTasks);

        // Assert
        using AssertionScope scope = new();
        _activeClientsManager.GetActiveClients().Count().Should().Be(clients.Count);
        _activeClientsManager.GetActiveClients().Should().BeEquivalentTo(clients);
    }

    [Test]
    public async Task RemoveAsync_RemovesClientAsync()
    {
        // Arrange
        var clientToRemove = Substitute.For<IKickPusherClient>();
        await _activeClientsManager.AddAsync(clientToRemove);

        var clientToKeep = Substitute.For<IKickPusherClient>();
        await _activeClientsManager.AddAsync(clientToKeep);

        // Act
        await _activeClientsManager.RemoveAsync(clientToRemove.ChannelId);

        // Assert
        const int activeClientCount = 1;

        using AssertionScope scope = new();
        _activeClientsManager.GetActiveClients().Count().Should().Be(activeClientCount);
        _activeClientsManager.GetActiveClients().Should().BeEquivalentTo([clientToKeep]);
    }

    [Test]
    [TestCase(1000)]
    [TestCase(20000)]
    public async Task RemoveAsync_RemovesClientsConcurrentlyAsync(int activeClientCount)
    {
        // Arrange
        var clientsToKeep = Enumerable.Range(0, activeClientCount)
            .Select(_ => Substitute.For<IKickPusherClient>())
            .ToList();

        var clientsToRemove = Enumerable.Range(0, activeClientCount)
            .Select(_ => Substitute.For<IKickPusherClient>())
            .ToList();

        var clientsToKeepAddTasks = clientsToKeep
            .Select(c => _activeClientsManager.AddAsync(c));
        var clientsToRemoveAddTasks = clientsToRemove
            .Select(c => _activeClientsManager.AddAsync(c));

        await Task.WhenAll([.. clientsToKeepAddTasks, .. clientsToRemoveAddTasks]);

        // Act
        var clientsToRemoveRemoveTasks = clientsToRemove
            .Select(c => _activeClientsManager.RemoveAsync(c.ChannelId));
        await Task.WhenAll(clientsToRemoveRemoveTasks);

        // Assert
        using AssertionScope scope = new();
        _activeClientsManager.GetActiveClients().Count().Should().Be(clientsToKeep.Count);
        _activeClientsManager.GetActiveClients().Should().BeEquivalentTo(clientsToKeep);
    }
}