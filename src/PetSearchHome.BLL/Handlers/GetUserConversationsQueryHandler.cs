using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Handlers;

public class GetUserConversationsQueryHandler : IRequestHandler<GetUserConversationsQuery, IReadOnlyList<ConversationPreviewDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;

    public GetUserConversationsQueryHandler(IConversationRepository conversationRepository, IUserRepository userRepository)
    {
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
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

            result.Add(new ConversationPreviewDto
            {
                ConversationId = conv.Id,
                OtherParticipantName = otherUser.UserType == UserType.Shelter
                    ? otherUser.ShelterProfile.Name
                    : $"{otherUser.IndividualProfile.FirstName} {otherUser.IndividualProfile.LastName}",

                OtherParticipantAvatarUrl = otherUser.UserType == UserType.Shelter
                    ? otherUser.ShelterProfile.LogoUrl
                    : otherUser.IndividualProfile.PhotoUrl,

                LastMessage = "...",
                LastMessageAt = conv.LastMessageAt
            });
        }

        return result.OrderByDescending(r => r.LastMessageAt).ToList();
    }
}