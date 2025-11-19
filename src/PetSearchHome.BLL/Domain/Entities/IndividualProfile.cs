namespace PetSearchHome.BLL.Domain.Entities;

public class IndividualProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? PhotoUrl { get; set; }

    public RegisteredUser User { get; set; } = null!;
}

