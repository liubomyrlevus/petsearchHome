using MediatR;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;

namespace PetSearchHome.BLL.Handlers;

public class GetReportsByStatusQueryHandler : IRequestHandler<GetReportsByStatusQuery, IReadOnlyList<ReportDto>>
{
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;

    public GetReportsByStatusQueryHandler(IReportRepository reportRepository, IUserRepository userRepository)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<ReportDto>> Handle(GetReportsByStatusQuery request, CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.GetByStatusAsync(request.Status, cancellationToken);
        var result = new List<ReportDto>();

        foreach (var report in reports)
        {
            var reporter = await _userRepository.GetByIdAsync(report.ReporterId, cancellationToken);

            result.Add(new ReportDto
            {
                Id = report.Id,
                ReporterId = report.ReporterId,
                ReporterInfo = reporter?.Email ?? "Unknown User",
                ReportedType = report.ReportedType,
                ReportedEntityId = report.ReportedEntityId,
                Reason = report.Reason,
                Description = report.Description,
                Status = report.Status,
                CreatedAt = report.CreatedAt,
                ResolvedAt = report.ResolvedAt
            });
        }

        return result;
    }
}

