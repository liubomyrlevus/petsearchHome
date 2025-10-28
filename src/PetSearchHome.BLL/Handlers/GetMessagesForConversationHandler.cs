using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Queries;

namespace PetSearchHome.BLL.Handlers;

public sealed class GetMessagesForConversationHandler : IRequestHandler<GetMessagesForConversationQuery, IReadOnlyList<Message>>
{
    private readonly IMessageRepository _messageRepository;

    public GetMessagesForConversationHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<IReadOnlyList<Message>> Handle(GetMessagesForConversationQuery request, CancellationToken cancellationToken)
    {
        if (request.ConversationId == Guid.Empty) return Array.Empty<Message>();

        var messages = await _messageRepository.GetByConversationAsync(request.ConversationId, cancellationToken);
        return messages;
    }
}
