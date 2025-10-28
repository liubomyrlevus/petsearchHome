using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetIndividualProfileByUserQuery(Guid UserId) : IRequest<IndividualProfile?>;
