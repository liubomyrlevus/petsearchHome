using MediatR;
namespace PetSearchHome.BLL.Commands;
public class AddFavoriteCommand : IRequest
{
    public int UserId { get; set; } 
    public int ListingId { get; set; }
}