using System.ComponentModel.DataAnnotations;

namespace PetSearchHome.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email є обов'язковим полем")]
        [EmailAddress(ErrorMessage = "Введіть коректну адресу електронної пошти")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль є обов'язковим полем")]
        [MinLength(6, ErrorMessage = "Пароль повинен містити щонайменше 6 символів")]
        public string Password { get; set; } = string.Empty;
    }
}
