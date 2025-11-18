using PetSearchHome.BLL.Domain.Enums;
namespace PetSearchHome.BLL.DTOs;
public class ListingCardDto
{
    public int Id { get; set; }
    public string? MainPhotoUrl { get; set; }
    public AnimalType AnimalType { get; set; }
    public string? Breed { get; set; }
    public int? AgeMonths { get; set; }
    public string? City { get; set; }
    public ListingStatus Status { get; set; }
}
// DTO для короткого відображення оголошення в загальному списку (каталозі).
