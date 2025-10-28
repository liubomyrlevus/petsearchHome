using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.Queries;

namespace PetSearchHome.BLL.Handlers;

public sealed class SearchListingsHandler : IRequestHandler<SearchListingsQuery, IReadOnlyList<Listing>>
{
    private readonly IListingRepository _listingRepository;

    public SearchListingsHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<IReadOnlyList<Listing>> Handle(SearchListingsQuery request, CancellationToken cancellationToken)
    {
        var listings = await _listingRepository.SearchAsync(request.SearchQuery, request.AnimalType, request.City, request.Status, cancellationToken);
        return listings;
    }
}
