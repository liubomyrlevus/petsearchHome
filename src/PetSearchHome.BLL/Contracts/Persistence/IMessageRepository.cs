using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Contracts.Persistence;

public interface IMessageRepository
{
    Task<IReadOnlyList<Message>> GetByConversationAsync(int conversationId, CancellationToken cancellationToken = default);
    Task<Message?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Message message, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(int messageId, CancellationToken cancellationToken = default);
}

