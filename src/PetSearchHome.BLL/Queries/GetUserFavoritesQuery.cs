using MediatR;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Queries;

// Запит на отримання списку улюблених оголошень для конкретного користувача.
public class GetUserFavoritesQuery : IRequest<IReadOnlyList<FavoriteListingDto>>
{
    public int UserId { get; set; }
}