using PetSearchHome.BLL.Domain.Enums;
namespace PetSearchHome.BLL.DTOs;

//Для адмінів
public class ReportDto
{
    public int Id { get; set; }
    public int ReporterId { get; set; }
    public string ReporterInfo { get; set; } = string.Empty;
    public ReportTargetType ReportedType { get; set; }
    public int ReportedEntityId { get; set; } 
    public string? Reason { get; set; }
    public string? Description { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}