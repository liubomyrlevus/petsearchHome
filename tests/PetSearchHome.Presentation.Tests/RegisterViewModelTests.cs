using Xunit;
using Moq;
using MediatR;
using Microsoft.AspNetCore.Components;
using PetSearchHome.ViewModels;
using PetSearchHome.Presentation.Services;
using PetSearchHome.BLL.Features.Auth.Commands;
using PetSearchHome.BLL.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace PetSearchHome.Presentation.Tests
{
    public class RegisterViewModelTests
    {
        [Fact]
        public async Task RegisterCommand_Should_LoginAndNavigate_When_RegistrationSuccess()
        {
            // 1. Arrange
            var mockMediator = new Mock<IMediator>();
            var mockNav = new Mock<NavigationManager>();
            var mockUser = new Mock<CurrentUserService>();

            // Налаштовуємо Mediator: "Реєстрація пройшла успішно"
            mockMediator.Setup(m => m.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new LoginResultDto
                        {
                            IsSuccess = true,
                            User = new UserProfileDto { Email = "new@user.com" }
                        });

            var viewModel = new RegisterViewModel(mockMediator.Object, mockNav.Object, mockUser.Object)
            {
                Email = "new@user.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FullName = "New User"
            };

            // 2. Act
            await viewModel.RegisterCommand.ExecuteAsync(null);

            // 3. Assert
            // Перевіряємо, що після реєстрації ми одразу залогінили юзера
            mockUser.Verify(u => u.Login(It.IsAny<UserDto>()), Times.Once);
        }
    }
}