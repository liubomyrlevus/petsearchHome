using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Contracts.Persistence;

public interface IFavoriteRepository
{
    Task<IReadOnlyList<Favorite>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid userId, int listingId, CancellationToken cancellationToken = default);

    Task AddAsync(Favorite favorite, CancellationToken cancellationToken = default);

    Task RemoveAsync(Guid userId, int listingId, CancellationToken cancellationToken = default);
}
