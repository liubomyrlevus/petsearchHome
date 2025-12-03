using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Microsoft.AspNetCore.Components;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.Presentation.Services;
using PetSearchHome.ViewModels;
using Xunit;

namespace PetSearchHome.Presentation.Tests;

public class LoginViewModelTests
{
    [Fact]
    public async Task LoginAsync_WhenSuccess_SetsUserAndNavigates()
    {
        var mediator = new Mock<IMediator>();
        var currentUser = new CurrentUserService();
        var nav = new TestNavigationManager();

        mediator.Setup(m => m.Send(It.IsAny<LoginQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResultDto
            {
                IsSuccess = true,
                User = new UserProfileDto
                {
                    Id = 5,
                    Email = "admin@test.com",
                    IsAdmin = true
                },
                Token = "token"
            });

        var vm = new LoginViewModel(mediator.Object, nav, currentUser)
        {
            Email = "admin@test.com",
            Password = "secret",
            RememberMe = true
        };

        await vm.LoginCommand.ExecuteAsync(null);

        Assert.True(currentUser.IsLoggedIn);
        Assert.True(currentUser.IsAdmin);
        Assert.Equal("admin@test.com", currentUser.UserEmail);
        Assert.Equal("/home", nav.NavigatedTo);
        Assert.True(nav.WasForceLoad);
        Assert.True(string.IsNullOrWhiteSpace(vm.ErrorMessage));
    }

    [Fact]
    public async Task LoginAsync_WhenWrongPassword_SetsErrorAndClearsPassword()
    {
        var mediator = new Mock<IMediator>();
        var currentUser = new CurrentUserService();
        var nav = new TestNavigationManager();

        mediator.Setup(m => m.Send(It.IsAny<LoginQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResultDto
            {
                IsSuccess = false,
                Error = "Невірний email або пароль."
            });

        var vm = new LoginViewModel(mediator.Object, nav, currentUser)
        {
            Email = "user@test.com",
            Password = "wrong123"
        };

        await vm.LoginCommand.ExecuteAsync(null);

        Assert.False(currentUser.IsLoggedIn);
        Assert.Equal("Невірний email або пароль.", vm.ErrorMessage);
        Assert.Equal(string.Empty, vm.Password);
        Assert.Null(nav.NavigatedTo);
    }

    [Fact]
    public async Task LoginAsync_WhenValidationFails_DoesNotCallMediator()
    {
        var mediator = new Mock<IMediator>(MockBehavior.Strict);
        var currentUser = new CurrentUserService();
        var nav = new TestNavigationManager();

        var vm = new LoginViewModel(mediator.Object, nav, currentUser)
        {
            Email = "",
            Password = ""
        };

        await vm.LoginCommand.ExecuteAsync(null);

        mediator.Verify(m => m.Send(It.IsAny<LoginQuery>(), It.IsAny<CancellationToken>()), Times.Never);
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
