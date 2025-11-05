using MediatR;
using PetSearchHome.BLL.DTOs;
namespace PetSearchHome.BLL.Queries;
public class GetConversationMessagesQuery : IRequest<IReadOnlyList<MessageDto>>
{
    public int ConversationId { get; set; } 
}