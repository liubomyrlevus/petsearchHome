using Microsoft.AspNetCore.Components;
using MediatR;
using PetSearchHome.ViewModels;
using PetSearchHome.BLL.Features.Auth.Commands.Login;

// ❗ ПРОБЛЕМА 1 (Namespace): Виправлено відповідно до шляху ...Presentation/Components/Pages
namespace PetSearchHome.Presentation.Components.Pages
{
    public partial class LoginPage : ComponentBase
    {
        [Inject]
        // ❗ ПРОБЛЕМА 2 (Nullability): Додано "= default!;"
        private IMediator Mediator { get; set; } = default!;

        [Inject]
        // ❗ ПРОБЛЕМА 2 (Nullability): Додано "= default!;"
        private NavigationManager NavManager { get; set; } = default!;

        // Властивість 'LoginViewModel', як очікує Учасник 5
        protected LoginViewModel LoginModel { get; set; } = new LoginViewModel();

        // Властивість 'ErrorMessage', як очікує Учасник 5
        // ❗ ПРОБЛЕМА 2 (Nullability): Додано '?', щоб дозволити null
        protected string? ErrorMessage { get; set; }

        // Метод 'HandleLoginSubmit', як очікує Учасник 5
        protected async Task HandleLoginSubmit()
        {
            ErrorMessage = null; // Тепер це коректно

            // ❗ ПРОБЛЕМА 3 (Конструктор): Змінено з { } на ( )
            var command = new LoginUserCommand(LoginModel.Email, LoginModel.Password);

            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            {
                NavManager.NavigateTo("/");
            }
            else
            {
                ErrorMessage = result.Error;
            }
        }

        // Метод 'GoToRegister', як очікує Учасник 5
        protected void GoToRegister()
        {
            NavManager.NavigateTo("/register");
        }

        // Метод 'ContinueAsGuest', як очікує Учасник 5
        protected void ContinueAsGuest()
        {
            NavManager.NavigateTo("/");
        }
    }
}