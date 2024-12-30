using System.Linq.Expressions;

using FluentAssertions.Execution;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using Wsrc.Core.Interfaces;
using Wsrc.Core.Interfaces.Mappings;
using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Core.Services.Kick;
using Wsrc.Domain.Entities;
using Wsrc.Domain.Models;
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
    private IConsumerServiceAcknowledger _acknowledger;
    private IMapper _mapper;

    private KickChatMessageBatchSavingService _service;

    [SetUp]
    public void SetUp()
    {
        _senderRepository = Substitute.For<IAsyncRepository<Sender>>();
        _messageRepository = Substitute.For<IAsyncRepository<Message>>();
        _acknowledger = Substitute.For<IConsumerServiceAcknowledger>();
        _mapper = new MapperMock().SetupMapper();

        (_serviceProvider, _serviceScopeFactory) = new ServiceProviderMock().SetupMock();

        _serviceProvider.GetService(typeof(IAsyncRepository<Message>)).Returns(_messageRepository);
        _serviceProvider.GetService(typeof(IAsyncRepository<Sender>)).Returns(_senderRepository);

        _service = new KickChatMessageBatchSavingService(_serviceScopeFactory, _mapper, _acknowledger);
    }

    [Test]
    public async Task HandleMessageAsync_CreatesSender_WhenSenderDoesntExist()
    {
        // Arrange
        var kickChatMessage = new KickChatMessageProvider().Create();

        var sender = new SenderProvider().Create();

        _mapper.KickChatMessageMapper.ToSender(kickChatMessage).Returns(sender);

        var parsedKickChatMessage = new ParsedKickChatMessageProvider()
            .Create(
                m => m.KickChatMessage = kickChatMessage
            );

        // Act
        await _service.HandleMessageAsync(parsedKickChatMessage);

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

        var parsedKickChatMessage = new ParsedKickChatMessageProvider().Create();

        // Act
        await _service.HandleMessageAsync(parsedKickChatMessage);

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

        const string ulongDeliveryTagString = "123";
        var parsedKickChatMessage = new ParsedKickChatMessageProvider()
            .Create(m =>
            {
                m.KickChatMessage = kickChatMessage;
                m.MessageEnvelope = new MessageEnvelopeProvider()
                    .Create(
                        me => me.Headers["DeliveryTag"] = ulongDeliveryTagString
                    );
            });

        // Act
        const int batchSize = 100;

        for (var i = 0; i < batchSize; i++)
        {
            await _service.HandleMessageAsync(parsedKickChatMessage);
        }

        // Assert
        await _messageRepository
            .Received()
            .AddRangeAsync(Arg.Is<List<Message>>(c => c.Count == batchSize));
    }

    [Test]
    [TestCase(1)]
    [TestCase(99)]
    public async Task HandleMessageAsync_DontSaveBatchMessages_WhenBatchSizeIsNotReached(int messageCount)
    {
        // Arrange
        var parsedKickChatMessage = new ParsedKickChatMessageProvider().Create();

        // Act 
        for (var i = 0; i < messageCount; i++)
        {
            await _service.HandleMessageAsync(parsedKickChatMessage);
        }

        // Assert
        await _messageRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<List<Message>>());
    }

    [Test]
    public async Task HandleMessageAsync_AcknowledgesMessages_AfterBatchIsSaved()
    {
        // Arrange
        var message = new MessageProvider().Create();

        _mapper.KickChatMessageMapper.ToMessage(Arg.Any<KickChatMessage>()).Returns(message);

        const int batchSize = 100;
        var batchParsedKickChatMessages = Enumerable
            .Range(0, batchSize)
            .Select((_, i) => new ParsedKickChatMessageProvider()
                .CreateWithDeliveryTag((ulong)i))
            .ToList();

        // Act
        foreach (var parsedKickChatMessage in batchParsedKickChatMessages)
        {
            await _service.HandleMessageAsync(parsedKickChatMessage);
        }

        // Assert
        using var assertionScope = new AssertionScope();

        await _messageRepository
            .Received()
            .AddRangeAsync(Arg.Is<List<Message>>(c => c.Count == batchSize));

        var messageEnvelopes = batchParsedKickChatMessages
            .Select(kcm => kcm.MessageEnvelope);

        foreach (var messageEnvelope in messageEnvelopes)
        {
            await _acknowledger
                .Received()
                .AcknowledgeAsync(messageEnvelope);
        }
    }

    [Test]
    [TestCase(1)]
    [TestCase(99)]
    public async Task HandleMessageAsync_DoesntAcknowledgeMessages_WhenBatchSizeIsNotReached(int messageCount)
    {
        // Arrange
        var parsedKickChatMessage = new ParsedKickChatMessageProvider()
            .Create();

        // Act 
        for (var i = 0; i < messageCount; i++)
        {
            await _service.HandleMessageAsync(parsedKickChatMessage);
        }

        // Assert
        using var assertionScope = new AssertionScope();

        await _messageRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<List<Message>>());

        await _acknowledger
            .DidNotReceive()
            .AcknowledgeAsync(Arg.Any<MessageEnvelope>());
    }
}