using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Queries;

namespace PetSearchHome.BLL.Handlers;

public sealed class GetListingByIdHandler : IRequestHandler<GetListingByIdQuery, Listing?>
{
    private readonly IListingRepository _listingRepository;

    public GetListingByIdHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<Listing?> Handle(GetListingByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0) return null;

        var listing = await _listingRepository.GetByIdAsync(request.Id, cancellationToken);
        return listing;
    }
}
