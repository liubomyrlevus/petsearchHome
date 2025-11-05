using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
namespace PetSearchHome.BLL.Handlers;
public class GetConversationMessagesQueryHandler : IRequestHandler<GetConversationMessagesQuery, IReadOnlyList<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    public GetConversationMessagesQueryHandler(IMessageRepository messageRepository) { _messageRepository = messageRepository; }
    public async Task<IReadOnlyList<MessageDto>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetByConversationAsync(request.ConversationId, cancellationToken);
        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            Content = m.Content,
            CreatedAt = m.CreatedAt,
            IsRead = m.IsRead
        })
            .OrderBy(m => m.CreatedAt)
            .ToList();
    }
}