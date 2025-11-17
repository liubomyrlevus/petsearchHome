using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.Domain.Entities; 

namespace PetSearchHome.BLL.Handlers;
public class GetUserConversationsQueryHandler : IRequestHandler<GetUserConversationsQuery, IReadOnlyList<ConversationPreviewDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository; 

    public GetUserConversationsQueryHandler(IConversationRepository conversationRepository, IUserRepository userRepository, IMessageRepository messageRepository) // Додано
    {
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
        _messageRepository = messageRepository; 
    }

    public async Task<IReadOnlyList<ConversationPreviewDto>> Handle(GetUserConversationsQuery request, CancellationToken cancellationToken)
    {
        var conversations = await _conversationRepository.GetByUserAsync(request.UserId, cancellationToken);
        var result = new List<ConversationPreviewDto>();


        foreach (var conv in conversations)
        {
            var otherUserId = conv.User1Id == request.UserId ? conv.User2Id : conv.User1Id;
            var otherUser = await _userRepository.GetByIdAsync(otherUserId, cancellationToken);
            if (otherUser == null) continue;

            // ОНОВЛЕНО: Отримуємо останнє повідомлення
            var messages = await _messageRepository.GetByConversationAsync(conv.Id, cancellationToken);
            var lastMessage = messages.LastOrDefault();

            result.Add(new ConversationPreviewDto
            {
                ConversationId = conv.Id,
                OtherParticipantId = otherUser.Id,
                OtherParticipantName = otherUser.UserType == UserType.shelter ? otherUser.ShelterProfile!.Name : $"{otherUser.IndividualProfile!.FirstName} {otherUser.IndividualProfile!.LastName}",
                OtherParticipantAvatarUrl = otherUser.UserType == UserType.shelter ? otherUser.ShelterProfile!.LogoUrl : otherUser.IndividualProfile!.PhotoUrl,
                LastMessage = lastMessage?.Content ?? "Повідомлень ще немає.",
                LastMessageAt = conv.LastMessageAt
            });
        }
        return result.OrderByDescending(r => r.LastMessageAt).ToList();
    }
}