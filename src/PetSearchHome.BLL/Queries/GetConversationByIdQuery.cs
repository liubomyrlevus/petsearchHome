using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetConversationByIdQuery(Guid Id) : IRequest<Conversation?>;
