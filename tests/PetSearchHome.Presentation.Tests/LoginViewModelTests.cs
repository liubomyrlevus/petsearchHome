using MediatR;
using Microsoft.AspNetCore.Components;
using Moq;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries; // ⚠️ Переконайся, що тут саме той LoginQuery, що у ViewModel
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
        // ✅ ТЕСТ 1: Перевірка Clean Code (Всі параметри конструктора)
        [Fact]
        public void Constructor_Should_ThrowArgumentNullException_When_AnyDependencyIsNull()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockNav = new Mock<NavigationManager>();
            var mockUser = new Mock<CurrentUserService>();

            // Act & Assert
            // 1. Перевірка Mediator
            Assert.Throws<ArgumentNullException>(() =>
                new LoginViewModel(null, mockNav.Object, mockUser.Object));

            // 2. Перевірка NavigationManager
            Assert.Throws<ArgumentNullException>(() =>
                new LoginViewModel(mockMediator.Object, null, mockUser.Object));

            // 3. Перевірка CurrentUserService
            Assert.Throws<ArgumentNullException>(() =>
                new LoginViewModel(mockMediator.Object, mockNav.Object, null));
        }

        // ✅ ТЕСТ 2: Успішний вхід
        [Fact]
        public async Task LoginCommand_Should_Navigate_When_LoginSuccess()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockNav = new Mock<NavigationManager>();
            var mockUser = new Mock<CurrentUserService>();

            // Налаштовуємо успішну відповідь від BLL
            mockMediator.Setup(m => m.Send(It.IsAny<LoginQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new LoginResultDto
                        {
                            IsSuccess = true,
                            User = new UserProfileDto { Email = "test@test.com", Id = 1, IsAdmin = false }
                        });

            var viewModel = new LoginViewModel(mockMediator.Object, mockNav.Object, mockUser.Object)
            {
                Email = "test@test.com",
                Password = "password"
            };

            // Act
            await viewModel.LoginCommand.ExecuteAsync(null);

            // Assert
            // Перевіряємо, що ми викликали сервіс для збереження юзера
            mockUser.Verify(u => u.Login(It.IsAny<UserDto>()), Times.Once);
        }

        // ✅ ТЕСТ 3: Логічна помилка (Невірний пароль) - ЦЬОГО НЕ ВИСТАЧАЛО
        [Fact]
        public async Task LoginCommand_Should_SetErrorMessage_When_LoginFailed_WrongPassword()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockNav = new Mock<NavigationManager>();
            var mockUser = new Mock<CurrentUserService>();

            // Налаштовуємо НЕуспішну відповідь (IsSuccess = false)
            mockMediator.Setup(m => m.Send(It.IsAny<LoginQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new LoginResultDto
                        {
                            IsSuccess = false,
                            Error = "Невірний пароль"
                        });

            var viewModel = new LoginViewModel(mockMediator.Object, mockNav.Object, mockUser.Object)
            {
                Email = "test@test.com",
                Password = "wrong_password"
            };

            // Act
            await viewModel.LoginCommand.ExecuteAsync(null);

            // Assert
            // Перевіряємо, що повідомлення про помилку з'явилося у ViewModel
            Assert.Equal("Невірний пароль", viewModel.ErrorMessage);
            // Перевіряємо, що вхід НЕ відбувся
            mockUser.Verify(u => u.Login(It.IsAny<UserDto>()), Times.Never);
        }

        // ✅ ТЕСТ 4: Технічна помилка (Exception)
        [Fact]
        public async Task LoginCommand_Should_SetErrorMessage_When_ExceptionOccurs()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockNav = new Mock<NavigationManager>();
            var mockUser = new Mock<CurrentUserService>();

            // Симулюємо "падіння" бази даних
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