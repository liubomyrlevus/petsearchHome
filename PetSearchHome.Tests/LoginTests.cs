using Moq;
using Xunit;
using PetSearchHome.BLL.Handlers;
using PetSearchHome.BLL.Queries;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Services.Authentication;
using PetSearchHome.BLL.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace PetSearchHome.Tests;

public class LoginTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordHasher> _hasherMock;
    private readonly Mock<IJwtTokenGenerator> _tokenGenMock;
    private readonly Mock<ISessionRepository> _sessionRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly LoginQueryHandler _handler;

    public LoginTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _hasherMock = new Mock<IPasswordHasher>();
        _tokenGenMock = new Mock<IJwtTokenGenerator>();
        _sessionRepoMock = new Mock<ISessionRepository>();
        _uowMock = new Mock<IUnitOfWork>();

        _handler = new LoginQueryHandler(
            _userRepoMock.Object,
            _hasherMock.Object,
            _tokenGenMock.Object,
            _sessionRepoMock.Object,
            _uowMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Password_Is_Invalid()
    {
        var query = new LoginQuery { Email = "user@test.com", Password = "wrongpassword" };
        var user = new RegisteredUser { Id = 1, Email = "user@test.com", PasswordHash = "real_hash", IsActive = true };

        _userRepoMock.Setup(r => r.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _hasherMock.Setup(h => h.Verify(query.Password, user.PasswordHash))
            .Returns(false); 


        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(query, CancellationToken.None));

        Assert.Equal("Invalid email or password.", exception.Message);
    }
}