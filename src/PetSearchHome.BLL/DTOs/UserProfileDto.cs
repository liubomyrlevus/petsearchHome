using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.DTOs;

public class UserProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public IndividualProfileDto? IndividualProfile { get; set; }
    public ShelterProfileDto? ShelterProfile { get; set; }
}

public class IndividualProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? PhotoUrl { get; set; }
}

public class ShelterProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
}

