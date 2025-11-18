using MediatR;

namespace PetSearchHome.BLL.Commands;

public class RemoveFavoriteCommand : IRequest
{
    public int UserId { get; set; }
    public int ListingId { get; set; }
}

