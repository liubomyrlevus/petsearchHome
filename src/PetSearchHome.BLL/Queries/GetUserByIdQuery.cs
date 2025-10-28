using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<RegisteredUser?>;
