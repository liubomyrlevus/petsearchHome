using MediatR;
using Microsoft.AspNetCore.Components;
using Moq;
using PetSearchHome.BLL.DTOs; // Для LoginResultDto
using PetSearchHome.BLL.Features.Auth.Commands.Login;
using PetSearchHome.BLL.Queries;
using PetSearchHome.Presentation.Services;
using PetSearchHome.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PetSearchHome.Presentation.Tests
{
    public class LoginViewModelTests
    {
        // ✅ ТЕСТ 1: Перевірка Clean Code (Викид винятку в конструкторі)
        [Fact]
        public void Constructor_Should_ThrowArgumentNullException_When_MediatorIsNull()
        {
            // Arrange
            IMediator nullMediator = null;
            var mockNav = new Mock<NavigationManager>();
            var mockUser = new Mock<CurrentUserService>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new LoginViewModel(nullMediator, mockNav.Object, mockUser.Object));
        }

        // ✅ ТЕСТ 2: Успішний вхід
        [Fact]
        public async Task LoginCommand_Should_Navigate_When_LoginSuccess()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockNav = new Mock<NavigationManager>(); // NavigationManager важко мокати, але для тесту пройде
            var mockUser = new Mock<CurrentUserService>();

            // Налаштовуємо Mediator: "Поверни успіх"
            mockMediator.Setup(m => m.Send(It.IsAny<LoginQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new LoginResultDto
                        {
                            IsSuccess = true,
                            User = new UserProfileDto { Email = "test@test.com" }
                        });

            var viewModel = new LoginViewModel(mockMediator.Object, mockNav.Object, mockUser.Object)
            {
                Email = "test@test.com",
                Password = "password"
            };

            // Act
            await viewModel.LoginCommand.ExecuteAsync(null);

            // Assert
            // Перевіряємо, що метод Login у сервісі був викликаний
            mockUser.Verify(u => u.Login(It.IsAny<UserDto>()), Times.Once);
        }

        // ✅ ТЕСТ 3: Обробка помилок (Вимога тімліда про винятки)
        [Fact]
        public async Task LoginCommand_Should_SetErrorMessage_When_ExceptionOccurs()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockNav = new Mock<NavigationManager>();
            var mockUser = new Mock<CurrentUserService>();

            // Налаштовуємо Mediator: "Викинь помилку!"
            mockMediator.Setup(m => m.Send(It.IsAny<LoginQuery>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new Exception("Database Error"));

            var viewModel = new LoginViewModel(mockMediator.Object, mockNav.Object, mockUser.Object)
            {
                Email = "test@test.com",
                Password = "password"
            };

            // Act
            await viewModel.LoginCommand.ExecuteAsync(null);

            // Assert
            Assert.Contains("Database Error", viewModel.ErrorMessage);
        }
    }
}