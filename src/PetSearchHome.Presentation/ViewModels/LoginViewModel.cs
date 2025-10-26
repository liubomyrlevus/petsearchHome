using System.ComponentModel.DataAnnotations;

namespace PetSearchHome.ViewModels // ❗ Namespace PetSearchHome.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Неправильний формат email")]
        public string Email { get; set; } = ""; // 👈 Додано = ""

        [Required(ErrorMessage = "Пароль є обов'язковим")]
        public string Password { get; set; } = ""; // 👈 Додано = ""
    }
}