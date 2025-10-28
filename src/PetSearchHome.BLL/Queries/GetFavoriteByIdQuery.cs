using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetFavoriteByUserAndListingQuery(Guid UserId, int ListingId) : IRequest<bool>;
