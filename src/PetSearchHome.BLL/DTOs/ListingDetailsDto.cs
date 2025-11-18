using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.DTOs;

public class ListingDetailsDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public AnimalType AnimalType { get; set; }
    public string? Breed { get; set; }
    public int? AgeMonths { get; set; }
    public AnimalSex? Sex { get; set; }
    public AnimalSize? Size { get; set; }
    public string? Color { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Description { get; set; }
    public string? SpecialNeeds { get; set; }
    public int ViewsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public HealthInfoDto? HealthInfo { get; set; }
    public List<string> PhotoUrls { get; set; } = new List<string>();
}

