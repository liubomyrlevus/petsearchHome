using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Domain.Entities;

public class Report
{
    public int Id { get; set; }

    public int ReporterId { get; set; }

    public ReportTargetType ReportedType { get; set; }

    public int ReportedEntityId { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ReportStatus Status { get; set; } = ReportStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ResolvedAt { get; set; }
}
