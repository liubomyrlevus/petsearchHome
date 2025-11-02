using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.DTOs;

// DTO для повної, детальної сторінки оголошення.
public class ListingDetailsDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public AnimalType AnimalType { get; set; }
    public string Breed { get; set; } = string.Empty;
    public int AgeMonths { get; set; }
    public AnimalSex Sex { get; set; }
    public AnimalSize Size { get; set; }
    public string Color { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? SpecialNeeds { get; set; }
    public int ViewsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public HealthInfoDto? HealthInfo { get; set; }
    public List<string> PhotoUrls { get; set; } = new List<string>();
}