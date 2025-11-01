using MediatR;

namespace PetSearchHome.BLL.Commands;

public class DeleteListingCommand : IRequest
{
    public int ListingId { get; set; }
    public Guid UserId { get; set; } 
}