using PetSearchHome.DAL.Domain.Enums;

namespace PetSearchHome.DAL.Domain.Entities;

public class RegisteredUser
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserType UserType { get; set; } = UserType.unknown;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsAdmin { get; set; }

    public IndividualProfile? IndividualProfile { get; set; }
    public ShelterProfile? ShelterProfile { get; set; }
}

