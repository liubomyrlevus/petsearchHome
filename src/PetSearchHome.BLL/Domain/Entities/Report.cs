using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Domain.Entities;

public class Report
{
    public Guid Id { get; set; }

    public Guid ReporterId { get; set; }

    public ReportTargetType ReportedType { get; set; }

    public Guid ReportedEntityId { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ReportStatus Status { get; set; } = ReportStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ResolvedAt { get; set; }
}
