using MediatR;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
namespace PetSearchHome.BLL.Handlers;
public class GetUserFavoritesQueryHandler : IRequestHandler<GetUserFavoritesQuery, IReadOnlyList<FavoriteListingDto>>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IListingRepository _listingRepository;
    public GetUserFavoritesQueryHandler(IFavoriteRepository favoriteRepository, IListingRepository listingRepository)
    {
        _favoriteRepository = favoriteRepository; _listingRepository = listingRepository;
    }
    public async Task<IReadOnlyList<FavoriteListingDto>> Handle(GetUserFavoritesQuery request, CancellationToken cancellationToken)
    {
        var favorites = await _favoriteRepository.GetByUserAsync(request.UserId, cancellationToken);
        if (!favorites.Any()) { return new List<FavoriteListingDto>(); }
        var listingIds = favorites.Select(f => f.ListingId).ToList();
        var listings = await _listingRepository.GetByIdsAsync(listingIds, cancellationToken);
        return listings.Select(l => new FavoriteListingDto
        {
            ListingId = l.Id,
            AnimalType = l.AnimalType.ToString(),
            Breed = l.Breed,
            MainPhotoUrl = l.Photos.FirstOrDefault()?.Url
        }).ToList();
    }
}