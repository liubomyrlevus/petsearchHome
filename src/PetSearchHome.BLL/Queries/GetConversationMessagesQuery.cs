using MediatR;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Queries;

public class GetConversationMessagesQuery : IRequest<IReadOnlyList<MessageDto>>
{
    public Guid ConversationId { get; set; }

}