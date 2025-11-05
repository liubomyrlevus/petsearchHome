namespace PetSearchHome.BLL.Domain.Entities;
public class ShelterProfile
{
    public int Id { get; set; }
    public int UserId { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? AdditionalPhone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? FacebookUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? LogoUrl { get; set; }

    public RegisteredUser User { get; set; } = null!;
}