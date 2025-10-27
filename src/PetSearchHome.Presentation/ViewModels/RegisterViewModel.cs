using System.ComponentModel.DataAnnotations;

namespace PetSearchHome.ViewModels // ❗ Namespace PetSearchHome.ViewModels
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
        public string Email { get; set; } = ""; // 👈 Додано

        [Required(ErrorMessage = "Пароль є обов'язковим")]
        [MinLength(6, ErrorMessage = "Пароль має бути щонайменше 6 символів")]
        public string Password { get; set; } = ""; // 👈 Додано

        // --- Поля для "Приватна особа" ---
        [Required(ErrorMessage = "Введіть ім'я та прізвище")]
        public string FullName { get; set; } = ""; // 👈 Додано

        [Required(ErrorMessage = "Введіть телефон")]
        [Phone(ErrorMessage = "Неправильний формат телефону")]
        public string Phone { get; set; } = ""; // 👈 Додано

        [Required(ErrorMessage = "Введіть адресу (місто + район)")]
        public string Address { get; set; } = ""; // 👈 Додано

        public string AdditionalInfo { get; set; } = ""; // 👈 Додано

        // --- Поля для "Притулок" ---
        [Required(ErrorMessage = "Введіть назву притулку")]
        public string ShelterName { get; set; } = ""; // 👈 Додано

        [Required(ErrorMessage = "Введіть контактну особу")]
        public string ContactPerson { get; set; } = ""; // 👈 Додано

        public string ShelterAddress { get; set; } = ""; // 👈 Додано
        public string Description { get; set; } = ""; // 👈 Додано
        public string SocialLinks { get; set; } = ""; // 👈 Додано
    }
}