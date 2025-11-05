using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
namespace PetSearchHome.BLL.Handlers;
public class StartConversationCommandHandler : IRequestHandler<StartConversationCommand, int> // ОНОВЛЕНО
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUnitOfWork _unitOfWork;
    public StartConversationCommandHandler(IConversationRepository conversationRepository, IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository; _unitOfWork = unitOfWork;
    }
    public async Task<int> Handle(StartConversationCommand request, CancellationToken cancellationToken)
    {
        var existingConversation = await _conversationRepository.GetByParticipantsAsync(
            request.InitiatorUserId, request.ReceiverUserId, request.ListingId, cancellationToken);
        if (existingConversation != null) { return existingConversation.Id; }
        var newConversation = new Conversation
        {
            User1Id = request.InitiatorUserId,
            User2Id = request.ReceiverUserId,
            ListingId = request.ListingId,
            LastMessageAt = DateTime.UtcNow
        };
        await _conversationRepository.AddAsync(newConversation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return newConversation.Id;
    }
}