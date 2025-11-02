using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.DTOs;

// DTO для відображення профілю користувача.
public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserType UserType { get; set; }

    public IndividualProfileDto? IndividualProfile { get; set; }
    public ShelterProfileDto? ShelterProfile { get; set; }
}

public class IndividualProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string? AdditionalInfo { get; set; }
    public string? PhotoUrl { get; set; }
}

public class ShelterProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
}