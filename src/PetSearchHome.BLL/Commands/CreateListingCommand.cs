using MediatR;
using PetSearchHome.DAL.Domain.Enums;
using PetSearchHome.BLL.DTOs;
namespace PetSearchHome.BLL.Commands;
public class CreateListingCommand : IRequest<int>
{
    public int UserId { get; set; }
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
    public HealthInfoDto? HealthInfo { get; set; }
    public List<string> PhotoUrls { get; set; } = new List<string>();
}

// Команда для створення оголошення.
