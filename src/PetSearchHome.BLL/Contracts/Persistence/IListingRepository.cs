using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Contracts.Persistence;

public interface IListingRepository
{
    Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Listing>> GetByOwnerAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Listing>> SearchAsync(
        string? searchQuery,
        AnimalType? animalType,
        string? city,
        ListingStatus? status,
        CancellationToken cancellationToken = default);

    Task<int> AddAsync(Listing listing, CancellationToken cancellationToken = default);

    Task UpdateAsync(Listing listing, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
