using System.Linq.Expressions;

using FluentAssertions;
using FluentAssertions.Execution;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using Wsrc.Core.Interfaces;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Domain.Entities;
using Wsrc.Infrastructure.Services;
using Wsrc.Tests.Reusables.Mocks;
using Wsrc.Tests.Reusables.Providers;

namespace Wsrc.Tests.Unit.Infrastructure.Services;

[TestFixture]
public class ActiveKickPusherClientFactoryTests
{
    private IActiveClientsManager _activeClientsManager;
    private IServiceScopeFactory _serviceScopeFactory;
    private IKickPusherClientFactory _kickPusherClientFactory;
    private IServiceProvider _serviceProvider;

    private ActiveKickPusherClientFactory _activeKickPusherClientFactory;

    [SetUp]
    public void SetUp()
    {
        _activeClientsManager = Substitute.For<IActiveClientsManager>();
        (_serviceProvider, _serviceScopeFactory) = new ServiceProviderMock().SetupMock();
        _kickPusherClientFactory = Substitute.For<IKickPusherClientFactory>();

        _activeKickPusherClientFactory = new ActiveKickPusherClientFactory(
            _activeClientsManager,
            _serviceScopeFactory,
            _kickPusherClientFactory);
    }

    [Test]
    public async Task CreateAllClientsAsync_ReturnsKickPusherClientsForAllChannels()
    {
        // Arrange
        var channelRepository = Substitute.For<IAsyncRepository<Channel>>();

        var channels = new ChannelProvider().ProvideDefault();
        channelRepository.GetAllAsync().Returns(channels);

        _serviceProvider.GetService<IAsyncRepository<Channel>>().Returns(channelRepository);

        var createdClients = channels.Select(c => Substitute.For<IKickPusherClient>()).ToList();

        _kickPusherClientFactory.CreateClients(channels)
            .Returns(createdClients);

        // Act
        var clients = await _activeKickPusherClientFactory.CreateAllClientsAsync();

        // Assert
        var clientsList = clients.ToList();

        using var scope = new AssertionScope();
        clientsList.Count.Should().Be(channels.Count);
        clientsList.Should().BeEquivalentTo(createdClients);
    }

    [Test]
    public async Task CreateDisconnectedClientsAsync_ReturnsDisconnectedKickPusherClients()
    {
        // Arrange
        var channelRepository = Substitute.For<IAsyncRepository<Channel>>();
        _serviceProvider.GetService<IAsyncRepository<Channel>>().Returns(channelRepository);

        var channels = new ChannelProvider().ProvideDefault();
        var activeChannels = channels.Take(1).ToList();

        var activeClients = activeChannels.Select(c => Substitute.For<IKickPusherClient>());

        _activeClientsManager.GetActiveClients().Returns(activeClients);

        var inactiveChannels = channels.Skip(1).ToList();

        channelRepository.GetWhereAsync(Arg.Any<Expression<Func<Channel, bool>>>())
            .Returns(inactiveChannels);

        _kickPusherClientFactory.CreateClients(inactiveChannels)
            .Returns(inactiveChannels.Select(c => Substitute.For<IKickPusherClient>()));

        // Act
        var disconnectedClients = await _activeKickPusherClientFactory
            .CreateDisconnectedClientsAsync();

        // Assert
        var disconnectedClientsList = disconnectedClients.ToList();

        using var scope = new AssertionScope();
        disconnectedClientsList.Count.Should().Be(inactiveChannels.Count);
    }
}