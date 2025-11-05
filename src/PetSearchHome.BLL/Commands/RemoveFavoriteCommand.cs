using MediatR;

namespace PetSearchHome.BLL.Commands;

// Видалення оголошення з улюблених.
public class RemoveFavoriteCommand : IRequest
{
    public int UserId { get; set; }
    public int ListingId { get; set; }
}