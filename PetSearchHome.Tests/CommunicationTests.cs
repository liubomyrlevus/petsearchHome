using Moq;
using Xunit;
using PetSearchHome.BLL.Handlers;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Queries;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.DAL.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

// Тести для "Розмов" та "Повідомлень"

namespace PetSearchHome.Tests;

public class CommunicationTests
{
    private readonly Mock<IConversationRepository> _convRepoMock;
    private readonly Mock<IMessageRepository> _msgRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;

    public CommunicationTests()
    {
        _convRepoMock = new Mock<IConversationRepository>();
        _msgRepoMock = new Mock<IMessageRepository>();
        _uowMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task StartConversation_Should_Create_New_If_Not_Exists()
    {
        var handler = new StartConversationCommandHandler(_convRepoMock.Object, _uowMock.Object);
        var command = new StartConversationCommand { InitiatorUserId = 1, ReceiverUserId = 2, ListingId = 5 };

        _convRepoMock.Setup(r => r.GetByParticipantsAsync(1, 2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null); // Немає

        await handler.Handle(command, CancellationToken.None);

        _convRepoMock.Verify(r => r.AddAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendMessage_Should_Add_Message_And_Update_Conversation()
    {
        var handler = new SendMessageCommandHandler(_msgRepoMock.Object, _convRepoMock.Object, _uowMock.Object);
        var command = new SendMessageCommand { ConversationId = 1, SenderId = 1, Content = "Hi" };
        var conversation = new Conversation { Id = 1 };

        _convRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(conversation);

        var result = await handler.Handle(command, CancellationToken.None);

        _msgRepoMock.Verify(r => r.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Once);
        _convRepoMock.Verify(r => r.UpdateAsync(conversation, It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal("Hi", result.Content);
    }

    [Fact]
    public async Task GetMessages_Should_Return_Ordered_List()
    {
        var handler = new GetConversationMessagesQueryHandler(_msgRepoMock.Object);
        var query = new GetConversationMessagesQuery { ConversationId = 1 };
        var messages = new List<Message>
        {
            new Message { Id = 1, Content = "Hello" },
            new Message { Id = 2, Content = "World" }
        };

        _msgRepoMock.Setup(r => r.GetByConversationAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(messages);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("Hello", result[0].Content);
    }
}