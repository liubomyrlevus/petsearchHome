using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.DTOs;

// DTO для короткого відображення оголошення в загальному списку (каталозі).
public class ListingCardDto
{
    public int Id { get; set; }
    public string? MainPhotoUrl { get; set; }
    public AnimalType AnimalType { get; set; }
    public string Breed { get; set; } = string.Empty;
    public int AgeMonths { get; set; }
    public string City { get; set; } = string.Empty;
}