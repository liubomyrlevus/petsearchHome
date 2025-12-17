using System.Collections.Generic;
using System.Threading;
using PetSearchHome.DAL.Domain.Entities;

namespace PetSearchHome.DAL.Contracts.Persistence;

public interface IUserRepository
{
    Task<RegisteredUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RegisteredUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(RegisteredUser user, CancellationToken cancellationToken = default);
    Task UpdateAsync(RegisteredUser user, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegisteredUser>> GetAllAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

