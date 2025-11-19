using MediatR;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Commands;

public class UpdateUserContactInfoCommand : IRequest<UserProfileDto>
{
    public int UserId { get; set; }
    public IndividualContactInfoUpdate? Individual { get; set; }
    public ShelterContactInfoUpdate? Shelter { get; set; }
}

public class IndividualContactInfoUpdate
{
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? AdditionalInfo { get; set; }
}

public class ShelterContactInfoUpdate
{
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
}
