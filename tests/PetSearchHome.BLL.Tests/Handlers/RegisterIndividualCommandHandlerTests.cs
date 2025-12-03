using FluentAssertions;
using Moq;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.Handlers;
using PetSearchHome.BLL.Services.Authentication;
using Xunit;

namespace PetSearchHome.BLL.Tests.Handlers;

public class RegisterIndividualCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtTokenGenerator> _jwt = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    private RegisterIndividualCommandHandler CreateHandler() =>
        new(_userRepository.Object, _passwordHasher.Object, _jwt.Object, _uow.Object);

    [Fact]
    public async Task Handle_Should_Throw_WhenEmailExists()
    {
        _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RegisteredUser());

        var handler = CreateHandler();
        var cmd = new RegisterIndividualCommand { Email = "test@test.com" };

        await Assert.ThrowsAsync<Exception>(() => handler.Handle(cmd, CancellationToken.None));

        _userRepository.Verify(r => r.AddAsync(It.IsAny<RegisteredUser>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_CreateUser_AndReturnToken()
    {
        _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RegisteredUser?)null);
        _passwordHasher.Setup(h => h.Hash("P@ssw0rd")).Returns("hashed");
        _jwt.Setup(j => j.GenerateToken(It.IsAny<RegisteredUser>())).Returns("jwt-token");

        RegisteredUser? addedUser = null;
        _userRepository.Setup(r => r.AddAsync(It.IsAny<RegisteredUser>(), It.IsAny<CancellationToken>()))
            .Callback<RegisteredUser, CancellationToken>((u, _) => addedUser = u)
            .Returns(Task.CompletedTask);

        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = CreateHandler();
        var cmd = new RegisterIndividualCommand
        {
            Email = "test@test.com",
            Password = "P@ssw0rd",
            FirstName = "Ivan",
            LastName = "Petrenko",
            Phone = "123",
            City = "Kyiv",
            District = "Obolon"
        };

        var result = await handler.Handle(cmd, CancellationToken.None);

        result.Token.Should().Be("jwt-token");
        result.User.Email.Should().Be(cmd.Email);
        result.User.UserType.Should().Be(UserType.individual);
        result.User.IndividualProfile.Should().NotBeNull();
        result.User.IndividualProfile!.FirstName.Should().Be("Ivan");

        addedUser.Should().NotBeNull();
        addedUser!.PasswordHash.Should().Be("hashed");
        addedUser.UserType.Should().Be(UserType.individual);
        _userRepository.Verify(r => r.AddAsync(It.IsAny<RegisteredUser>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
