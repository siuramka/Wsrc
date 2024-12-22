using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Wsrc.Core.Interfaces.Mappings;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Core.Services.Kick;
using Wsrc.Domain;
using Wsrc.Domain.Entities;
using Wsrc.Domain.Models;
using Wsrc.Tests.Reusables.Mocks;

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

        var sender = new Sender
        {
            Id = 1,
            Username = "User1",
            Slug = "user1",
        };

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

        var sender = new Sender
        {
            Id = 1,
            Username = "User1",
            Slug = "user1",
        };

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
