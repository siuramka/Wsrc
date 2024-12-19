using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Wsrc.Core.Interfaces.Mappings;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Core.Services.Kick;
using Wsrc.Domain;
using Wsrc.Domain.Entities;
using Wsrc.Domain.Models;

namespace Wsrc.Tests.Core.Services.Kick;

[TestFixture]
public class KickChatMessageBatchSavingServiceTests
{
    private IServiceScopeFactory _serviceScopeFactory;
    private IServiceScope _serviceScope;
    private IServiceProvider _serviceProvider;
    private IAsyncRepository<Sender> _senderRepository;
    private IAsyncRepository<Message> _messageRepository;
    private IKickChatMessageMapper _kickChatMessageMapper;

    private KickChatMessageBatchSavingService _service;

    [SetUp]
    public void SetUp()
    {
        _serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        _serviceScope = Substitute.For<IServiceScope>();
        _serviceProvider = Substitute.For<IServiceProvider>();
        _senderRepository = Substitute.For<IAsyncRepository<Sender>>();
        _messageRepository = Substitute.For<IAsyncRepository<Message>>();
        _kickChatMessageMapper = Substitute.For<IKickChatMessageMapper>();

        _serviceScopeFactory.CreateScope().Returns(_serviceScope);
        _serviceScope.ServiceProvider.Returns(_serviceProvider);

        _serviceProvider.GetService(typeof(IAsyncRepository<Message>)).Returns(_messageRepository);
        _serviceProvider.GetService(typeof(IAsyncRepository<Sender>)).Returns(_senderRepository);

        _service = new KickChatMessageBatchSavingService(_serviceScopeFactory, _kickChatMessageMapper);
    }

    [TearDown]
    public void TearDown()
    {
        _serviceScope.Dispose();
    }

    [Test]
    public async Task HandleMessageAsync_SavesBatchMessages_WhenBatchSizeIsReached()
    {
        // Arrange
        var kickChatMessage = new KickChatMessage
        {
            Data = new KickChatMessageChatInfo
            {
                ChatroomId = 1,
                Content = "Hello, World!",
                CreatedAt = DateTime.UtcNow,
                KickChatMessageSender = new KickChatMessageSender
                {
                    Id = 1,
                    Username = "User1",
                    Slug = "user1",
                },
            },
        };

        var message = new Message
        {
            ChatroomId = 1,
            Content = "Hello, World",
            Timestamp = DateTime.UtcNow,
            SenderId = 1,
        };

        _kickChatMessageMapper.ToMessage(kickChatMessage).Returns(message);

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
        var kickChatMessage = new KickChatMessage
        {
            Data = new KickChatMessageChatInfo
            {
                ChatroomId = 1,
                Content = "Hello, World!",
                CreatedAt = DateTime.UtcNow,
                KickChatMessageSender = new KickChatMessageSender
                {
                    Id = 1,
                    Username = "User1",
                    Slug = "user1",
                },
            },
        };

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
