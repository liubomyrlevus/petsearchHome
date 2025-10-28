using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Queries;

namespace PetSearchHome.BLL.Handlers;

public sealed class GetReportByIdHandler : IRequestHandler<GetReportByIdQuery, Report?>
{
    private readonly IReportRepository _reportRepository;

    public GetReportByIdHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<Report?> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty) return null;

        var r = await _reportRepository.GetByIdAsync(request.Id, cancellationToken);
        return r;
    }
}
