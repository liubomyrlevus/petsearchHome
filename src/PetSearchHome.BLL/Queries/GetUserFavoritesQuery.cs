using MediatR;
using PetSearchHome.BLL.DTOs;
namespace PetSearchHome.BLL.Queries;
public class GetUserFavoritesQuery : IRequest<IReadOnlyList<FavoriteListingDto>>
{
    public int UserId { get; set; } 
}

// Запит на отримання списку улюблених оголошень для конкретного користувача.
