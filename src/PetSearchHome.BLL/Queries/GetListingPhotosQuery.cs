using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetListingPhotosQuery(int ListingId) : IRequest<IReadOnlyList<ListingPhoto>>;
