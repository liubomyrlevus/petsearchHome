using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Contracts.Persistence;

public interface IUserRepository
{

    Task<RegisteredUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RegisteredUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(RegisteredUser user, CancellationToken cancellationToken = default);
    Task UpdateAsync(RegisteredUser user, CancellationToken cancellationToken = default);
}