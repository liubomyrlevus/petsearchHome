using MediatR;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Commands;

// Команда для оновлення існуючого оголошення.
public class UpdateListingCommand : IRequest
{
    public int Id { get; set; } // ID оголошення, яке редагується
    public int UserId { get; set; } // ID користувача, який редагує
    public AnimalType AnimalType { get; set; }
    public string? Breed { get; set; }
    public int AgeMonths { get; set; }
    public AnimalSex Sex { get; set; }
    public AnimalSize Size { get; set; }
    public string Color { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? SpecialNeeds { get; set; }
    public HealthInfoDto? HealthInfo { get; set; }
    public List<string> PhotoUrls { get; set; } = new List<string>();
}