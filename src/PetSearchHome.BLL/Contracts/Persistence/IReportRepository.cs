using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Contracts.Persistence;

public interface IReportRepository
{
    Task<IReadOnlyList<Report>> GetByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default);

    Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Report report, CancellationToken cancellationToken = default);

    Task UpdateAsync(Report report, CancellationToken cancellationToken = default);
}
