using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Commands;

public sealed record CreateFavoriteCommand(Guid UserId, int ListingId) : IRequest<Favorite?>;
