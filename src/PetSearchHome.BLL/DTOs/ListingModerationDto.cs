using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.DTOs;

public class ListingModerationDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string OwnerEmail { get; set; } = string.Empty;
    public AnimalType AnimalType { get; set; }
  public string? Breed { get; set; }
    public int? AgeMonths { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Description { get; set; }
    public ListingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
    public string? ModerationComment { get; set; }
    public string? PrimaryPhotoUrl { get; set; }
}
