using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Contracts.Persistence;

public interface IMessageRepository
{
    Task<IReadOnlyList<Message>> GetByConversationAsync(Guid conversationId, CancellationToken cancellationToken = default);

    Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Message message, CancellationToken cancellationToken = default);

    Task MarkAsReadAsync(Guid messageId, CancellationToken cancellationToken = default);
}
