using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Contracts.Persistence;

public interface IReviewRepository
{
    Task<IReadOnlyList<Review>> GetForUserAsync(Guid reviewedUserId, CancellationToken cancellationToken = default);

    Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Review review, CancellationToken cancellationToken = default);

    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
