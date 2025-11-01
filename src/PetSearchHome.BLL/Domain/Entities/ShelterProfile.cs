namespace PetSearchHome.BLL.Domain.Entities;

public class ShelterProfile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ContactPerson { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string? AdditionalPhone { get; set; }

    public string Address { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? FacebookUrl { get; set; }

    public string? InstagramUrl { get; set; }

    public string? WebsiteUrl { get; set; }

    public string? LogoUrl { get; set; }

    public RegisteredUser User { get; set; } = null!;
}
