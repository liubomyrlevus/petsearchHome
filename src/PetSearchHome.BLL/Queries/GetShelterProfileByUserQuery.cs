using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetShelterProfileByUserQuery(Guid UserId) : IRequest<ShelterProfile?>;
