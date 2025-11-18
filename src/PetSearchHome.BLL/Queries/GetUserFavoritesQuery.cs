using MediatR;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Queries;

public class GetUserFavoritesQuery : IRequest<IReadOnlyList<FavoriteListingDto>>
{
    public int UserId { get; set; }
}

