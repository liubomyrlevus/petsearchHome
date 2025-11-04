using Microsoft.AspNetCore.Components;
using PetSearchHome.ViewModels;

// ❗ ПРОБЛЕМА 1 (Namespace): Виправлено відповідно до шляху ...Presentation/Components/Pages
namespace PetSearchHome.Presentation.Components.Pages
{
    public partial class LoginPage : ComponentBase
    {
        // Посилання на навігацію Blazor
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        // Властивість 'LoginViewModel', як очікує Учасник 5
        protected LoginViewModel LoginModel { get; set; } = new LoginViewModel();

        // Властивість 'ErrorMessage', як очікує Учасник 5
        // ❗ ПРОБЛЕМА 2 (Nullability): Додано '?', щоб дозволити null
        protected string? ErrorMessage { get; set; }

        // Метод 'HandleLoginSubmit', як очікує Учасник 5
        protected Task HandleLoginSubmit()
        {
            ErrorMessage = null; // Тепер це коректно

            // TODO: інтегрувати реальну авторизацію через BLL або API.
            NavManager.NavigateTo("/home");

            return Task.CompletedTask;
        }

        // Метод 'GoToRegister', як очікує Учасник 5
        protected void GoToRegister()
        {
            NavManager.NavigateTo("/register");
        }

        // Метод 'ContinueAsGuest', як очікує Учасник 5
        protected void ContinueAsGuest()
        {
            NavManager.NavigateTo("/home");
        }
    }
}
