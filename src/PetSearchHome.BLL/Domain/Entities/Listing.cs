using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Domain.Entities;

public class Listing
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public AnimalType AnimalType { get; set; } = AnimalType.unknown;
    public string? Breed { get; set; }
    public int? AgeMonths { get; set; }
    public AnimalSex? Sex { get; set; } = AnimalSex.unknown;
    public AnimalSize? Size { get; set; } = AnimalSize.unknown;
    public string? Color { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Description { get; set; }
    public string? SpecialNeeds { get; set; }
    public ListingStatus Status { get; set; } = ListingStatus.draft;
    public int ViewsCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? ModerationComment { get; set; }

    public RegisteredUser User { get; set; } = null!;
    public HealthInfo? HealthInfo { get; set; }
    public ICollection<ListingPhoto> Photos { get; set; } = new List<ListingPhoto>();
}

