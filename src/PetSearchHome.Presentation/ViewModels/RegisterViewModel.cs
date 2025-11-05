using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PetSearchHome.ViewModels // ❗ Namespace PetSearchHome.ViewModels
{
    // Цей enum потрібен для перемикача "Притулок" / "Приватна особа"
    public enum UserType
    {
        PrivatePerson,
        Shelter
    }

    public class RegisterViewModel : IValidatableObject
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
        public string FullName { get; set; } = ""; // 👈 Додано

        [Phone(ErrorMessage = "Неправильний формат телефону")]
        public string Phone { get; set; } = ""; // 👈 Додано

        public string Address { get; set; } = ""; // 👈 Додано

        public string AdditionalInfo { get; set; } = ""; // 👈 Додано

        // --- Поля для "Притулок" ---
        public string ShelterName { get; set; } = ""; // 👈 Додано

        public string ContactPerson { get; set; } = ""; // 👈 Додано

        public string ShelterAddress { get; set; } = ""; // 👈 Додано
        public string Description { get; set; } = ""; // 👈 Додано
        public string SocialLinks { get; set; } = ""; // 👈 Додано

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AccountType == UserType.PrivatePerson)
            {
                if (string.IsNullOrWhiteSpace(FullName))
                    yield return new ValidationResult("Введіть ім'я та прізвище", new[] { nameof(FullName) });

                if (string.IsNullOrWhiteSpace(Phone))
                    yield return new ValidationResult("Введіть телефон", new[] { nameof(Phone) });

                if (string.IsNullOrWhiteSpace(Address))
                    yield return new ValidationResult("Введіть адресу (місто + район)", new[] { nameof(Address) });
            }
            else if (AccountType == UserType.Shelter)
            {
                if (string.IsNullOrWhiteSpace(ShelterName))
                    yield return new ValidationResult("Введіть назву притулку", new[] { nameof(ShelterName) });

                if (string.IsNullOrWhiteSpace(ContactPerson))
                    yield return new ValidationResult("Введіть контактну особу", new[] { nameof(ContactPerson) });
            }
        }
    }
}