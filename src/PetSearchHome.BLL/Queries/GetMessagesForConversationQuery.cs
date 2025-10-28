using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetMessagesForConversationQuery(Guid ConversationId) : IRequest<IReadOnlyList<Message>>;
