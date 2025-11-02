namespace PetSearchHome.BLL.DTOs;

// Об'єкт для передачі даних про улюблене оголошення на UI.
public class FavoriteListingDto
{
    public int ListingId { get; set; }
    public string AnimalType { get; set; } // Наприклад, "Кіт" чи "Собака"
    public string Breed { get; set; } // Порода
    public string? MainPhotoUrl { get; set; } // URL головного фото
}