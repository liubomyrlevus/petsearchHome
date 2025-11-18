using MediatR;
using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Commands;

public class ModerateListingCommand : IRequest
{
    public int ListingId { get; set; }
    public int ModeratorId { get; set; }
    public ListingStatus NewStatus { get; set; }
    public string? ModerationComment { get; set; }
}
