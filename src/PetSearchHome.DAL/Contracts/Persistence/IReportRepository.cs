using PetSearchHome.DAL.Domain.Entities;
using PetSearchHome.DAL.Domain.Enums;

namespace PetSearchHome.DAL.Contracts.Persistence;

public interface IReportRepository
{
    Task<IReadOnlyList<Report>> GetByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default);
    Task<Report?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Report report, CancellationToken cancellationToken = default);
    Task UpdateAsync(Report report, CancellationToken cancellationToken = default);
}

