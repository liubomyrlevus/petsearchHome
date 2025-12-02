using MediatR;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.DAL.Domain.Entities;
using System.Linq;

namespace PetSearchHome.BLL.Handlers;

public class SearchListingsQueryHandler : IRequestHandler<SearchListingsQuery, IReadOnlyList<ListingCardDto>>
{
    private readonly IListingRepository _listingRepository;

    public SearchListingsQueryHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<IReadOnlyList<ListingCardDto>> Handle(SearchListingsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Listing> listings;

        if (request.UserId.HasValue)
        {
            var userListings = await _listingRepository.GetByOwnerAsync(request.UserId.Value, cancellationToken);
            listings = request.Status.HasValue
                ? userListings.Where(l => l.Status == request.Status.Value)
                : userListings;
        }
        else
        {
            listings = await _listingRepository.SearchAsync(
                request.SearchQuery,
                request.AnimalType,
                request.City,
                request.Status,
                cancellationToken);
        }

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
