using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Queries;

namespace PetSearchHome.BLL.Handlers;

public sealed class GetListingsByOwnerHandler : IRequestHandler<GetListingsByOwnerQuery, IReadOnlyList<Listing>>
{
    private readonly IListingRepository _listingRepository;

    public GetListingsByOwnerHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<IReadOnlyList<Listing>> Handle(GetListingsByOwnerQuery request, CancellationToken cancellationToken)
    {
        if (request.OwnerId == Guid.Empty) return Array.Empty<Listing>();

        var list = await _listingRepository.GetByOwnerAsync(request.OwnerId, cancellationToken);
        return list;
    }
}
