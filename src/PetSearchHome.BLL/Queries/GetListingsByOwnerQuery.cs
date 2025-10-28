using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetListingsByOwnerQuery(Guid OwnerId) : IRequest<IReadOnlyList<Listing>>;
