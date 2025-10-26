using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Contracts.Persistence;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Conversation>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Conversation?> GetByParticipantsAsync(Guid user1Id, Guid user2Id, int? listingId, CancellationToken cancellationToken = default);

    Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default);

    Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default);
}
