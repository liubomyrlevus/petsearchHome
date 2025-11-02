namespace PetSearchHome.BLL.Domain.Entities;

public class IndividualProfile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public string? AdditionalInfo { get; set; }

    public string? PhotoUrl { get; set; }

    public RegisteredUser User { get; set; } = null!;
}
