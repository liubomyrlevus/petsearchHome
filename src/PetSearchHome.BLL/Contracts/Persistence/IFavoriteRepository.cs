using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Contracts.Persistence;

public interface IFavoriteRepository
{
  
    Task<IReadOnlyList<Favorite>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(int userId, int listingId, CancellationToken cancellationToken = default);
    Task AddAsync(Favorite favorite, CancellationToken cancellationToken = default);
    
    Task RemoveAsync(int userId, int listingId, CancellationToken cancellationToken = default);
}