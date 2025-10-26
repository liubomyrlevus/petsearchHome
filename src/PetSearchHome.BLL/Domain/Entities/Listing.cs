using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Domain.Entities;

public class Listing
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public AnimalType AnimalType { get; set; } = AnimalType.Unknown;

    public string Breed { get; set; } = string.Empty;

    public int AgeMonths { get; set; }

    public AnimalSex Sex { get; set; } = AnimalSex.Unknown;

    public AnimalSize Size { get; set; } = AnimalSize.Unknown;

    public string Color { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? SpecialNeeds { get; set; }

    public ListingStatus Status { get; set; } = ListingStatus.Pending;

    public int ViewsCount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public string? ModerationComment { get; set; }

    public HealthInfo? HealthInfo { get; set; }

    public List<ListingPhoto> Photos { get; set; } = [];
}
