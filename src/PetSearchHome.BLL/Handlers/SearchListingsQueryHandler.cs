using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.BLL.Domain.Enums;
namespace PetSearchHome.BLL.Handlers;
public class SearchListingsQueryHandler : IRequestHandler<SearchListingsQuery, IReadOnlyList<ListingCardDto>>
{
    private readonly IListingRepository _listingRepository;
    public SearchListingsQueryHandler(IListingRepository listingRepository) { _listingRepository = listingRepository; }
    public async Task<IReadOnlyList<ListingCardDto>> Handle(SearchListingsQuery request, CancellationToken cancellationToken)
    {
        var listings = await _listingRepository.SearchAsync(
            request.SearchQuery,
            request.AnimalType,
            request.City,
            request.Status,
            request.UserId,
            cancellationToken
        );
        return listings.Select(l => new ListingCardDto
        {
            Id = l.Id,
            MainPhotoUrl = l.Photos.FirstOrDefault()?.Url,
            AnimalType = l.AnimalType,
            Breed = l.Breed,
            AgeMonths = l.AgeMonths,
            City = l.City,
            Status = l.Status
        }).ToList();
    }
}
