using Moq;
using Xunit;
using PetSearchHome.BLL.Handlers;
using PetSearchHome.BLL.Commands;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.BLL.Services.Authentication;
using PetSearchHome.DAL.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace PetSearchHome.Tests;

public class RegisterIndividualTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordHasher> _hasherMock;
    private readonly Mock<IJwtTokenGenerator> _tokenMock;
    private readonly Mock<IUnitOfWork> _uowMock;

 
    private readonly RegisterIndividualCommandHandler _handler;

    public RegisterIndividualTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _hasherMock = new Mock<IPasswordHasher>();
        _tokenMock = new Mock<IJwtTokenGenerator>();
        _uowMock = new Mock<IUnitOfWork>();

        
        _handler = new RegisterIndividualCommandHandler(
            _userRepoMock.Object,
            _hasherMock.Object,
            _tokenMock.Object,
            _uowMock.Object
        );
    }

    [Fact]
    public async Task Handle_Should_Register_User_When_Email_Is_Unique()
    {

        var command = new RegisterIndividualCommand
        {
            Email = "new@test.com",
            Password = "password123",
            FirstName = "Test",
            LastName = "User"
        };

        _userRepoMock.Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RegisteredUser?)null);

        _hasherMock.Setup(h => h.Hash(command.Password)).Returns("hashed_password");
        _tokenMock.Setup(t => t.GenerateToken(It.IsAny<RegisteredUser>())).Returns("fake_token");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("fake_token", result.Token);

        _userRepoMock.Verify(repo => repo.AddAsync(It.IsAny<RegisteredUser>(), It.IsAny<CancellationToken>()), Times.Once);

        _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Email_Is_Taken()
    {
        var command = new RegisterIndividualCommand { Email = "busy@test.com", Password = "123" };

        _userRepoMock.Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RegisteredUser { Email = command.Email });

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Email is already taken.", exception.Message);

        _userRepoMock.Verify(repo => repo.AddAsync(It.IsAny<RegisteredUser>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}