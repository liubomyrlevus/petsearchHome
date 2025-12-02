using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.DAL.Contracts.Persistence;
namespace PetSearchHome.BLL.Handlers;
public class UpdateReportStatusCommandHandler : IRequestHandler<UpdateReportStatusCommand>
{
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateReportStatusCommandHandler(IReportRepository reportRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    { _reportRepository = reportRepository; _userRepository = userRepository; _unitOfWork = unitOfWork; }
    public async Task<Unit> Handle(UpdateReportStatusCommand request, CancellationToken cancellationToken)
    {
        var moderator = await _userRepository.GetByIdAsync(request.ModeratorId, cancellationToken);
        var isAdminEffective = moderator != null && (moderator.IsAdmin || moderator.UserType == UserType.shelter);
        if (!isAdminEffective)
        {
            throw new Exception("User is not authorized to moderate reports.");
        }
        var report = await _reportRepository.GetByIdAsync(request.ReportId, cancellationToken);
        if (report == null) { throw new Exception("Report not found."); }
        report.Status = request.NewStatus;
        if (request.NewStatus == ReportStatus.confirmed || request.NewStatus == ReportStatus.rejected)
        {
            report.ResolvedAt = DateTime.UtcNow;
        }
        await _reportRepository.UpdateAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
