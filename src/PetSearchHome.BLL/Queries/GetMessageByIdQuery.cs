using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetMessageByIdQuery(Guid Id) : IRequest<Message?>;
