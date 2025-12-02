using FluentAssertions;
using Moq;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.DAL.Domain.Entities;
using PetSearchHome.DAL.Domain.Enums;
using PetSearchHome.BLL.Features.Auth.Commands.Login;
using PetSearchHome.BLL.Services.Authentication;
using Xunit;

namespace PetSearchHome.BLL.Tests.Features.Auth.Commands.Login;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private LoginUserCommandHandler CreateHandler() =>
        new(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenGeneratorMock.Object,
            _unitOfWorkMock.Object);

    [Fact]
    public async Task Handle_Should_ReturnError_WhenUserNotFound()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RegisteredUser?)null);

        var handler = CreateHandler();
        var command = new LoginUserCommand("test@test.com", "password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Неправильний email або password.");
        _unitOfWorkMock.Verify(uw => uw.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenPasswordIsInvalid()
    {
        // Arrange
        var user = new RegisteredUser
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            PasswordHash = "hash"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(h => h.Verify("password123", user.PasswordHash))
            .Returns(false);

        var handler = CreateHandler();
        var command = new LoginUserCommand(user.Email, "password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Неправильний email або password.");
        _unitOfWorkMock.Verify(uw => uw.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenUserInactive()
    {
        // Arrange
        var user = new RegisteredUser
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            PasswordHash = "hash",
            UserType = UserType.Individual,
            IsActive = false
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(h => h.Verify(It.IsAny<string>(), user.PasswordHash))
            .Returns(true);

        var handler = CreateHandler();
        var command = new LoginUserCommand(user.Email, "password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Ваш акаунт деактивовано. Зверніться у підтримку.");
        _unitOfWorkMock.Verify(uw => uw.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new RegisteredUser
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            PasswordHash = "hash",
            UserType = UserType.Individual,
            IsActive = true
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(h => h.Verify("password123", user.PasswordHash))
            .Returns(true);

        _jwtTokenGeneratorMock
            .Setup(g => g.GenerateToken(user))
            .Returns("jwt-token");

        var handler = CreateHandler();
        var command = new LoginUserCommand(user.Email, "password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Token.Should().Be("jwt-token");
        result.Error.Should().BeNull();

        _userRepositoryMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uw => uw.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
