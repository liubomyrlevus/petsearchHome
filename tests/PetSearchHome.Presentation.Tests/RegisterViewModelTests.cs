using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Microsoft.AspNetCore.Components;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.Presentation.Services;
using PetSearchHome.ViewModels;
using Xunit;

namespace PetSearchHome.Presentation.Tests;

public class RegisterViewModelTests
{
    [Fact]
    public async Task RegisterAsync_WhenIndividualSuccess_LogsInAndNavigates()
    {
        var mediator = new Mock<IMediator>();
        var nav = new TestNavigationManager();
        var currentUser = new CurrentUserService();

        mediator.Setup(m => m.Send(It.IsAny<IRequest<LoginResultDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IRequest<LoginResultDto> req, CancellationToken _) =>
            {
                Assert.IsType<RegisterIndividualCommand>(req);
                return new LoginResultDto
                {
                    IsSuccess = true,
                    User = new UserProfileDto { Id = 7, Email = "user@test.com", IsAdmin = false },
                    Token = "token"
                };
            });

        var vm = new RegisterViewModel(mediator.Object, nav, currentUser)
        {
            SelectedUserType = "Individual",
            Email = "user@test.com",
            Password = "password123",
            ConfirmPassword = "password123",
            FirstName = "Ivan",
            LastName = "Petrenko"
        };

        await vm.RegisterCommand.ExecuteAsync(null);

        mediator.Verify(m => m.Send(It.IsAny<IRequest<LoginResultDto>>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(currentUser.IsLoggedIn, $"LoggedIn:{currentUser.IsLoggedIn}, Error:{vm.ErrorMessage}");
        Assert.False(currentUser.IsAdmin);
        Assert.Equal("user@test.com", currentUser.UserEmail);
        Assert.True(string.IsNullOrWhiteSpace(vm.ErrorMessage), vm.ErrorMessage);
    }

    [Fact]
    public async Task RegisterAsync_WhenShelterSuccess_LogsInAndNavigates()
    {
        var mediator = new Mock<IMediator>();
        var nav = new TestNavigationManager();
        var currentUser = new CurrentUserService();

        mediator.Setup(m => m.Send(It.IsAny<IRequest<LoginResultDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IRequest<LoginResultDto> req, CancellationToken _) =>
            {
                Assert.IsType<RegisterShelterCommand>(req);
                return new LoginResultDto
                {
                    IsSuccess = true,
                    User = new UserProfileDto { Id = 8, Email = "shelter@test.com", IsAdmin = true },
                    Token = "token"
                };
            });

        var vm = new RegisterViewModel(mediator.Object, nav, currentUser)
        {
            SelectedUserType = "Shelter",
            Email = "shelter@test.com",
            Password = "password123",
            ConfirmPassword = "password123",
            ShelterName = "Happy Pets",
            ContactPerson = "Jane Doe"
        };

        await vm.RegisterCommand.ExecuteAsync(null);

        mediator.Verify(m => m.Send(It.IsAny<IRequest<LoginResultDto>>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(currentUser.IsLoggedIn, $"LoggedIn:{currentUser.IsLoggedIn}, Error:{vm.ErrorMessage}");
        Assert.True(currentUser.IsAdmin);
        Assert.Equal("shelter@test.com", currentUser.UserEmail);
        Assert.True(string.IsNullOrWhiteSpace(vm.ErrorMessage), vm.ErrorMessage);
    }

    [Fact]
    public async Task RegisterAsync_WhenRegistrationFails_SetsError()
    {
        var mediator = new Mock<IMediator>();
        var nav = new TestNavigationManager();
        var currentUser = new CurrentUserService();

        mediator.Setup(m => m.Send(It.IsAny<IRequest<LoginResultDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IRequest<LoginResultDto> req, CancellationToken _) =>
            {
                Assert.IsType<RegisterIndividualCommand>(req);
                return new LoginResultDto
                {
                    IsSuccess = false,
                    Error = "Email вже зайнятий"
                };
            });

        var vm = new RegisterViewModel(mediator.Object, nav, currentUser)
        {
            SelectedUserType = "Individual",
            Email = "taken@test.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        await vm.RegisterCommand.ExecuteAsync(null);

        Assert.Equal("Email вже зайнятий", vm.ErrorMessage);
        Assert.False(currentUser.IsLoggedIn);
        Assert.Null(nav.NavigatedTo);
    }

    [Fact]
    public async Task RegisterAsync_WhenValidationFails_DoesNotSendCommand()
    {
        var mediator = new Mock<IMediator>(MockBehavior.Strict);
        var nav = new TestNavigationManager();
        var currentUser = new CurrentUserService();

        var vm = new RegisterViewModel(mediator.Object, nav, currentUser)
        {
            SelectedUserType = "Individual",
            Email = "", // invalid
            Password = "short",
            ConfirmPassword = "diff"
        };

        await vm.RegisterCommand.ExecuteAsync(null);

        mediator.Verify(m => m.Send(It.IsAny<IRequest<LoginResultDto>>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.False(currentUser.IsLoggedIn);
        Assert.Null(nav.NavigatedTo);
    }

    private sealed class TestNavigationManager : NavigationManager
    {
        public string? NavigatedTo { get; private set; }
        public bool WasForceLoad { get; private set; }
        public bool WasReplace { get; private set; }

        public TestNavigationManager()
        {
            Initialize("http://localhost/", "http://localhost/");
        }

        protected override void NavigateToCore(string uri, bool forceLoad) => NavigateTo(uri, forceLoad);

        protected override void NavigateToCore(string uri, NavigationOptions options)
        {
            NavigatedTo = uri;
            WasForceLoad = options.ForceLoad;
            WasReplace = options.ReplaceHistoryEntry;
        }

        public new void NavigateTo(string uri, bool forceLoad = false, bool replace = false)
        {
            WasReplace = replace;
            NavigatedTo = uri;
            WasForceLoad = forceLoad;
        }
    }
}
