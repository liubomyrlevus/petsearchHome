using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.Queries;

namespace PetSearchHome.BLL.Handlers;

public sealed class GetReportsByStatusHandler : IRequestHandler<GetReportsByStatusQuery, IReadOnlyList<Report>>
{
    private readonly IReportRepository _reportRepository;

    public GetReportsByStatusHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<IReadOnlyList<Report>> Handle(GetReportsByStatusQuery request, CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.GetByStatusAsync(request.Status, cancellationToken);
        return reports;
    }
}
