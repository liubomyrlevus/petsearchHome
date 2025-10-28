using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetFavoritesForUserQuery(Guid UserId) : IRequest<IReadOnlyList<Favorite>>;
