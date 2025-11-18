using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Contracts.Persistence;

public interface IReviewRepository
{
    Task<IReadOnlyList<Review>> GetForUserAsync(int reviewedUserId, CancellationToken cancellationToken = default);
    Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

