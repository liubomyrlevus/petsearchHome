using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Commands;

public sealed record CreateConversationCommand(Guid User1Id, Guid User2Id, int? ListingId) : IRequest<Conversation?>;
