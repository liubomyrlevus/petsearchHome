using MediatR;
namespace PetSearchHome.BLL.Commands;
public class DeleteListingCommand : IRequest
{
    public int ListingId { get; set; }
    public int UserId { get; set; }
    public bool IsAdmin { get; set; }
}
