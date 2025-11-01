using MediatR;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Queries;

public class GetUserConversationsQuery : IRequest<IReadOnlyList<ConversationPreviewDto>>
{
    public Guid UserId { get; set; }
}