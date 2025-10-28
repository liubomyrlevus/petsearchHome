using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Queries;

namespace PetSearchHome.BLL.Handlers;

public sealed class GetFavoritesForUserHandler : IRequestHandler<GetFavoritesForUserQuery, IReadOnlyList<Favorite>>
{
    private readonly IFavoriteRepository _favoriteRepository;

    public GetFavoritesForUserHandler(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public async Task<IReadOnlyList<Favorite>> Handle(GetFavoritesForUserQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty) return Array.Empty<Favorite>();

        var favs = await _favoriteRepository.GetByUserAsync(request.UserId, cancellationToken);
        return favs;
    }
}
