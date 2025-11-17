using MediatR;
using PetSearchHome.BLL.Domain.Enums;
namespace PetSearchHome.BLL.Commands;
public class UpdateReportStatusCommand : IRequest
{
    public int ReportId { get; set; } 
    public ReportStatus NewStatus { get; set; }
    public int ModeratorId { get; set; } 
}