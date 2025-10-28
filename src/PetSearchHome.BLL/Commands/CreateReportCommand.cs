using MediatR;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Commands;

public sealed record CreateReportCommand(Guid ReporterId, ReportTargetType ReportedType, Guid ReportedEntityId, string Reason, string? Description) : IRequest<Report>;
