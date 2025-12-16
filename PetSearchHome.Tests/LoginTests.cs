using Moq;
using Xunit;
using PetSearchHome.BLL.Handlers;
using PetSearchHome.BLL.Queries;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.BLL.Services.Authentication;
using PetSearchHome.DAL.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System;
using PetSearchHome.DAL.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace PetSearchHome.Tests;

public class LoginTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordHasher> _hasherMock;
    private readonly Mock<IJwtTokenGenerator> _tokenGenMock;
    private readonly Mock<ISessionRepository> _sessionRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly LoginQueryHandler _handler;
    private readonly Mock<ILogger<LoginQueryHandler>> _loggerMock;

    public LoginTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _hasherMock = new Mock<IPasswordHasher>();
        _tokenGenMock = new Mock<IJwtTokenGenerator>();
        _sessionRepoMock = new Mock<ISessionRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<LoginQueryHandler>>();

        _handler = new LoginQueryHandler(
            _userRepoMock.Object,
            _hasherMock.Object,
            _tokenGenMock.Object,
            _sessionRepoMock.Object,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_Password_Is_Invalid()
    {
        var query = new LoginQuery { Email = "user@test.com", Password = "wrongpassword" };
        var user = new RegisteredUser { Id = 1, Email = "user@test.com", PasswordHash = "real_hash", IsActive = true };

        _userRepoMock.Setup(r => r.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _hasherMock.Setup(h => h.Verify(query.Password, user.PasswordHash))
            .Returns(false); 

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Невірний email або пароль.", result.Error);
        _sessionRepoMock.Verify(s => s.AddAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()), Times.Never);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_User_And_Create_Session_On_Success()
    {
        var query = new LoginQuery { Email = "admin@test.com", Password = "secret" };
        var user = new RegisteredUser
        {
            Id = 10,
            Email = query.Email,
            PasswordHash = "hashed",
            IsActive = true,
            UserType = UserType.individual
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _hasherMock.Setup(h => h.Verify(query.Password, user.PasswordHash)).Returns(true);
        _tokenGenMock.Setup(t => t.GenerateToken(user)).Returns("jwt-token");

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal("jwt-token", result.Token);

        _sessionRepoMock.Verify(s => s.AddAsync(
            It.Is<Session>(sess => sess.UserId == user.Id && !string.IsNullOrWhiteSpace(sess.SessionToken)),
            It.IsAny<CancellationToken>()),
            Times.Once);

        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
