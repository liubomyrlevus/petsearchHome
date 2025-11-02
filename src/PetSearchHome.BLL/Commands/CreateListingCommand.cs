using MediatR;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Commands;

// Команда для створення оголошення. Повертає ID створеного оголошення.
public class CreateListingCommand : IRequest<int>
{
    public Guid UserId { get; set; }
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
    public HealthInfoDto? HealthInfo { get; set; }
    // !!!! UI повинен передати сюди URL-и вже завантажених фото !!!!
    public List<string> PhotoUrls { get; set; } = new List<string>();
}