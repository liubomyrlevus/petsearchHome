using PetSearchHome.DAL.Domain.Entities;

namespace PetSearchHome.DAL.Contracts.Persistence;

public interface ISessionRepository
{
    Task<Session?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(Session session, CancellationToken cancellationToken = default);
    Task InvalidateAsync(Guid sessionId, CancellationToken cancellationToken = default); 
    Task UpdateAsync(Session session, CancellationToken cancellationToken = default);
}