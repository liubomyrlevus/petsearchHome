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

        [Required(ErrorMessage = "Введіть ім'я та прізвище")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "Введіть телефон")]
        [Phone(ErrorMessage = "Неправильний формат телефону")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Введіть адресу (місто + район)")]
        public string Address { get; set; }

        public string AdditionalInfo { get; set; }
        
        [Required(ErrorMessage = "Введіть назву притулку")]
        public string ShelterName { get; set; } 

        [Required(ErrorMessage = "Введіть контактну особу")]
        public string ContactPerson { get; set; } 

        public string ShelterAddress { get; set; } 
        public string Description { get; set; } 
        public string SocialLinks { get; set; } 
    }
}