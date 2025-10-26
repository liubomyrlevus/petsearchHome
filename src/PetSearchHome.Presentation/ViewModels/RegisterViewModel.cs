using System.ComponentModel.DataAnnotations;

namespace PetSearchHome.ViewModels
{
    // Цей enum потрібен для перемикача "Притулок" / "Приватна особа"
    public enum UserType
    {
        PrivatePerson,
        Shelter
    }

    public class RegisterViewModel
    {
        [Required]
        public UserType AccountType { get; set; } = UserType.PrivatePerson;

        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Неправильний формат email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль є обов'язковим")]
        [MinLength(6, ErrorMessage = "Пароль має бути щонайменше 6 символів")]
        public string Password { get; set; }

        [cite_start]// --- Поля для "Приватна особа" [cite: 164-169] ---
        // Використовуй 'Required' для полів, які є обов'язковими
        [Required(ErrorMessage = "Введіть ім'я та прізвище")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "Введіть телефон")]
        [Phone(ErrorMessage = "Неправильний формат телефону")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Введіть адресу (місто + район)")]
        public string Address { get; set; } // [cite: 168]

        public string AdditionalInfo { get; set; } // [cite: 169]

        [cite_start]// --- Поля для "Притулок" [cite: 156-163] ---
        // Тут логіка буде складнішою: ці поля будуть
        // обов'язковими, ТІЛЬКИ ЯКЩО AccountType == UserType.Shelter.
        // Для початку просто додай їх.
        
        [Required(ErrorMessage = "Введіть назву притулку")]
        public string ShelterName { get; set; } // [cite: 157]

        [Required(ErrorMessage = "Введіть контактну особу")]
        public string ContactPerson { get; set; } // [cite: 158]

        public string ShelterAddress { get; set; } // [cite: 161]
        public string Description { get; set; } // [cite: 162]
        public string SocialLinks { get; set; } // [cite: 163]
    }
}