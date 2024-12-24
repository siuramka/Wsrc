using System.Collections.Concurrent;
using System.Linq.Expressions;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using Wsrc.Core.Interfaces.Mappings;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Core.Services.Kick;
using Wsrc.Domain.Entities;
using Wsrc.Tests.Reusables.Mocks;
using Wsrc.Tests.Reusables.Providers;

namespace Wsrc.Tests.Unit.Core.Services.Kick;

[TestFixture]
public class KickChatMessageBatchSavingServiceTests
{
    private IServiceProvider _serviceProvider;
    private IServiceScopeFactory _serviceScopeFactory;

    private IAsyncRepository<Sender> _senderRepository;
    private IAsyncRepository<Message> _messageRepository;
    private IMapper _mapper;

    private KickChatMessageBatchSavingService _service;

    [SetUp]
    public void SetUp()
    {
        _senderRepository = Substitute.For<IAsyncRepository<Sender>>();
        _messageRepository = Substitute.For<IAsyncRepository<Message>>();
        _mapper = new MapperMock().SetupMapper();

        (_serviceProvider, _serviceScopeFactory) = new ServiceProviderMock().SetupMock();

        _serviceProvider.GetService(typeof(IAsyncRepository<Message>)).Returns(_messageRepository);
        _serviceProvider.GetService(typeof(IAsyncRepository<Sender>)).Returns(_senderRepository);

        _service = new KickChatMessageBatchSavingService(_serviceScopeFactory, _mapper);
    }

    [Test]
    public async Task HandleMessageAsync_CreatesSender_WhenSenderDoesntExist()
    {
        // Arrange
        var kickChatMessage = new KickChatMessageProvider().Create();

        var sender = new SenderProvider().Create();

        _mapper.KickChatMessageMapper.ToSender(kickChatMessage).Returns(sender);

        // Act
        await _service.HandleMessageAsync(kickChatMessage);

        // Assert
        await _senderRepository
            .Received()
            .AddAsync(sender);
    }

    [Test]
    public async Task HandleMessageAsync_DoesntCreateSender_WhenSenderExists()
    {
        // Arrange
        var kickChatMessage = new KickChatMessageProvider().Create();

        var sender = new SenderProvider().Create();

        _senderRepository
            .FirstOrDefaultAsync(Arg.Any<Expression<Func<Sender, bool>>>())
            .Returns(sender);

        // Act
        await _service.HandleMessageAsync(kickChatMessage);

        // Assert
        await _senderRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Sender>());
    }

    [Test]
    public async Task HandleMessageAsync_SavesBatchMessages_WhenBatchSizeIsReached()
    {
        // Arrange
        var kickChatMessage = new KickChatMessageProvider().Create();

        var message = new MessageProvider().Create();

        _mapper.KickChatMessageMapper.ToMessage(kickChatMessage).Returns(message);

        // Act
        const int batchSize = 100;

        for (var i = 0; i < batchSize; i++)
        {
            await _service.HandleMessageAsync(kickChatMessage);
        }

        // Assert
        await _messageRepository
            .Received()
            .AddRangeAsync(Arg.Is<List<Message>>(c => c.Count == batchSize));
    }

    [Test]
    [TestCase(99)]
    [TestCase(1)]
    public async Task HandleMessageAsync_DontSaveBatchMessages_WhenBatchSizeIsNotReached(int messageCount)
    {
        // Arrange
        var kickChatMessage = new KickChatMessageProvider().Create();

        // Act 
        for (var i = 0; i < messageCount; i++)
        {
            await _service.HandleMessageAsync(kickChatMessage);
        }

        // Assert
        await _messageRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<ConcurrentQueue<Message>>());
    }
}