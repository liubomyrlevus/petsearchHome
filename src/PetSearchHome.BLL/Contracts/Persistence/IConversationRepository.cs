using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Contracts.Persistence;

public interface IConversationRepository
{

    Task<Conversation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<Conversation?> GetByParticipantsAsync(int user1Id, int user2Id, int? listingId, CancellationToken cancellationToken = default);
    Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default);
}