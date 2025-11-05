using MediatR;
using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Commands;
// коли натискає на кнопку "Поскаржитися"
public class CreateReportCommand : IRequest<int>
{
    public int ReporterId { get; set; }
    public ReportTargetType ReportedType { get; set; }
    public int ReportedEntityId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
}