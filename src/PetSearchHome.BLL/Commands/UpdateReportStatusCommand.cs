using MediatR;
using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Commands;

public class UpdateReportStatusCommand : IRequest
{
    public Guid ReportId { get; set; }
    public ReportStatus NewStatus { get; set; }
    public Guid ModeratorId { get; set; } // Для перевірки прав модератора
}