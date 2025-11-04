using Microsoft.AspNetCore.Components;
using PetSearchHome.ViewModels;

namespace PetSearchHome.Presentation.Components.Pages
{
    public partial class LoginPage : ComponentBase
    {
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        protected LoginViewModel LoginModel { get; set; } = new();

        protected string? ErrorMessage { get; set; }

        protected Task HandleLoginSubmit()
        {
            ErrorMessage = null;
            NavManager.NavigateTo("/home", replace: true);
            return Task.CompletedTask;
        }

        protected void GoToRegister()
        {
            NavManager.NavigateTo("/register");
        }

        protected void ContinueAsGuest()
        {
            NavManager.NavigateTo("/home", replace: true);
        }
    }
}
