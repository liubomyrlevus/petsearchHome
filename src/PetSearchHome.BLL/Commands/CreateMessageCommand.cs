using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Commands;

public sealed record CreateMessageCommand(Guid ConversationId, Guid SenderId, string Content) : IRequest<Message?>;
