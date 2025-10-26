using Microsoft.AspNetCore.Components;
using MediatR; // 📍 БУДЕ ЧЕРВОНИМ (поки що)
using PetSearchHome.ViewModels; // 📍 Стане нормальним після Кроку 3
using PetSearchHome.BLL.Commands; // 📍 БУДЕ ЧЕРВОНИМ (поки що)

namespace PetSearchHome.Pages
{
    // Клас 'LoginPage', як очікує Учасник 5
    public partial class LoginPage : ComponentBase
    {
        [Inject]
        private IMediator Mediator { get; set; } // 📍 БУДЕ ЧЕРВОНИМ

        [Inject]
        private NavigationManager NavManager { get; set; }

        // Властивість 'LoginViewModel', як очікує Учасник 5
        protected LoginViewModel LoginModel { get; set; } = new LoginViewModel();
        
        // Властивість 'ErrorMessage', як очікує Учасник 5
        protected string ErrorMessage { get; set; }

        // Метод 'HandleLoginSubmit', як очікує Учасник 5
        protected async Task HandleLoginSubmit()
        {
            ErrorMessage = null; 
            var command = new LoginUserCommand // 📍 БУДЕ ЧЕРВОНИМ
            { 
                Email = LoginModel.Email, 
                Password = LoginModel.Password 
            };

            var result = await Mediator.Send(command); // 📍 БУДЕ ЧЕРВОНИМ

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