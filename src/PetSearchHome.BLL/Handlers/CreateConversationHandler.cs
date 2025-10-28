using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Handlers;

public sealed class CreateConversationHandler : IRequestHandler<CreateConversationCommand, Conversation?>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateConversationHandler(IConversationRepository conversationRepository, IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Conversation?> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        if (request.User1Id == Guid.Empty || request.User2Id == Guid.Empty) return null;

        // check existing
        var existing = await _conversationRepository.GetByParticipantsAsync(request.User1Id, request.User2Id, request.ListingId, cancellationToken);
        if (existing is not null) return existing;

        var conv = new Conversation
        {
            Id = Guid.NewGuid(),
            User1Id = request.User1Id,
            User2Id = request.User2Id,
            ListingId = request.ListingId,
            LastMessageAt = DateTime.UtcNow
        };

        await _conversationRepository.AddAsync(conv, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return conv;
    }
}
