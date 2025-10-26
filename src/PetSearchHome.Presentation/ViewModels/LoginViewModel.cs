// System.ComponentModel.DataAnnotations потрібен для валідації
using System.ComponentModel.DataAnnotations;

// Переконайся, що простір імен правильний
namespace PetSearchHome.ViewModels
{
    public class LoginViewModel
    {
        // Атрибут [Required] змусить Blazor перевірити, чи поле не пусте.
        // ErrorMessage - це текст, який побачить Учасник 5
        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Неправильний формат email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Пароль є обов'язковим")]
        public string Password { get; set; }
    }
}