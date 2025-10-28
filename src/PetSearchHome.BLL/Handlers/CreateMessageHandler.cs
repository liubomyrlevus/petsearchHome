using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Handlers;

public sealed class CreateMessageHandler : IRequestHandler<CreateMessageCommand, Message?>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMessageHandler(IMessageRepository messageRepository, IConversationRepository conversationRepository, IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Message?> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        if (request.ConversationId == Guid.Empty || request.SenderId == Guid.Empty || string.IsNullOrWhiteSpace(request.Content)) return null;

        var conv = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conv is null) return null;

        var msg = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            SenderId = request.SenderId,
            Content = request.Content.Trim(),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _messageRepository.AddAsync(msg, cancellationToken);

        conv.Messages.Add(msg);
        conv.LastMessageAt = DateTime.UtcNow;
        await _conversationRepository.UpdateAsync(conv, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return msg;
    }
}
