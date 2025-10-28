using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Handlers;

public sealed class CreateReportHandler : IRequestHandler<CreateReportCommand, Report>
{
    private readonly IReportRepository _reportRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReportHandler(IReportRepository reportRepository, IUnitOfWork unitOfWork)
    {
        _reportRepository = reportRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Report> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var report = new Report
        {
            Id = Guid.NewGuid(),
            ReporterId = request.ReporterId,
            ReportedType = request.ReportedType,
            ReportedEntityId = request.ReportedEntityId,
            Reason = request.Reason ?? string.Empty,
            Description = request.Description,
            Status = PetSearchHome.BLL.Domain.Enums.ReportStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _reportRepository.AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return report;
    }
}
