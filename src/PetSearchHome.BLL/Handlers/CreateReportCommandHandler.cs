using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;
namespace PetSearchHome.BLL.Handlers;
public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, int>
{
    private readonly IReportRepository _reportRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateReportCommandHandler(IReportRepository reportRepository, IUnitOfWork unitOfWork)
    { _reportRepository = reportRepository; _unitOfWork = unitOfWork; }
    public async Task<int> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var report = new Report
        {
            ReporterId = request.ReporterId,
            ReportedType = request.ReportedType,
            ReportedEntityId = request.ReportedEntityId,
            Reason = request.Reason,
            Description = request.Description,
            Status = ReportStatus.pending
        };
        await _reportRepository.AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return report.Id;
    }
}