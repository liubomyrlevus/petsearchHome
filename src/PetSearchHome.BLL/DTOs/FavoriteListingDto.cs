namespace PetSearchHome.BLL.DTOs;

// Об'єкт для передачі даних про улюблене оголошення на UI.

public class FavoriteListingDto
{
    public int ListingId { get; set; }
    public string AnimalType { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string? MainPhotoUrl { get; set; }
}