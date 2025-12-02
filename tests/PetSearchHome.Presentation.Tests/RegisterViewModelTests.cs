using Xunit;
using Moq;
using MediatR;
using Microsoft.AspNetCore.Components;
using PetSearchHome.ViewModels;
using PetSearchHome.Presentation.Services;
using PetSearchHome.BLL.Features.Auth.Commands.Register; // Перевір шлях
using PetSearchHome.BLL.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetSearchHome.Presentation.Tests
{
    public class RegisterViewModelTests
    {
        // ✅ ТЕСТ 1: Clean Code (Захист конструктора)
        [Fact]
        public void Constructor_Should_ThrowArgumentNullException_When_DependenciesAreNull()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockNav = new Mock<NavigationManager>();
            var mockUser = new Mock<CurrentUserService>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new RegisterViewModel(null, mockNav.Object, mockUser.Object));

            Assert.Throws<ArgumentNullException>(() =>
                new RegisterViewModel(mockMediator.Object, null, mockUser.Object));

            Assert.Throws<ArgumentNullException>(() =>
                new RegisterViewModel(mockMediator.Object, mockNav.Object, null));
        }

        // ✅ ТЕСТ 2: Успішна реєстрація ПРИВАТНОЇ ОСОБИ (Individual)
        [Fact]
        public async Task RegisterCommand_Should_SendIndividualCommand_And_Login_When_Success()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockNav = new Mock<NavigationManager>();
            var mockUser = new Mock<CurrentUserService>();

            // Налаштовуємо успішну відповідь для RegisterIndividualCommand
            mockMediator.Setup(m => m.Send(It.IsAny<RegisterIndividualCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new LoginResultDto
                        {
                            IsSuccess = true,
                            User = new UserProfileDto { Email = "user@test.com", Id = 1 }
                        });

            var viewModel = new RegisterViewModel(mockMediator.Object, mockNav.Object, mockUser.Object)
            {
                SelectedUserType = "Individual", // ВАЖЛИВО!
                Email = "user@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "Ivan",
                LastName = "Ivanov"
            };

            // Act
            await viewModel.RegisterCommand.ExecuteAsync(null);

            // Assert
            // 1. Перевіряємо, що відправилась саме RegisterIndividualCommand
            mockMediator.Verify(m => m.Send(It.IsAny<RegisterIndividualCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            // 2. Перевіряємо, що відбувся вхід
            mockUser.Verify(u => u.Login(It.IsAny<UserDto>()), Times.Once);
        }

        // ✅ ТЕСТ 3: Успішна реєстрація ПРИТУЛКУ (Shelter)
        [Fact]
        public async Task RegisterCommand_Should_SendShelterCommand_And_Login_When_Success()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockNav = new Mock<NavigationManager>();
            var mockUser = new Mock<CurrentUserService>();

            // Налаштовуємо успішну відповідь для RegisterShelterCommand
            mockMediator.Setup(m => m.Send(It.IsAny<RegisterShelterCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new LoginResultDto
                        {
                            IsSuccess = true,
                            User = new UserProfileDto { Email = "shelter@test.com", Id = 2 }
                        });

            var viewModel = new RegisterViewModel(mockMediator.Object, mockNav.Object, mockUser.Object)
            {
                SelectedUserType = "Shelter", // ВАЖЛИВО!
                Email = "shelter@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                ShelterName = "Best Shelter"
            };

            // Act
            await viewModel.RegisterCommand.ExecuteAsync(null);

            // Assert
            // Перевіряємо, що відправилась саме RegisterShelterCommand
            mockMediator.Verify(m => m.Send(It.IsAny<RegisterShelterCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mockUser.Verify(u => u.Login(It.IsAny<UserDto>()), Times.Once);
        }

        // ✅ ТЕСТ 4: Помилка реєстрації (User already exists)
        [Fact]
        public async Task RegisterCommand_Should_SetErrorMessage_When_RegistrationFails()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockNav = new Mock<NavigationManager>();
            var mockUser = new Mock<CurrentUserService>();

            mockMediator.Setup(m => m.Send(It.IsAny<RegisterIndividualCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new LoginResultDto
                        {
                            IsSuccess = false,
                            Error = "Користувач вже існує"
                        });

            var viewModel = new RegisterViewModel(mockMediator.Object, mockNav.Object, mockUser.Object)
            {
                SelectedUserType = "Individual",
                Email = "exists@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            // Act
            await viewModel.RegisterCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Користувач вже існує", viewModel.ErrorMessage);
            // Вхід НЕ мав відбутися
            mockUser.Verify(u => u.Login(It.IsAny<UserDto>()), Times.Never);
        }
    }
}