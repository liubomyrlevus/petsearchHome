using MediatR;

namespace PetSearchHome.BLL.Commands;

// Додавання оголошення в улюблені.
public class AddFavoriteCommand : IRequest
{
    public Guid UserId { get; set; }
    public int ListingId { get; set; }
}