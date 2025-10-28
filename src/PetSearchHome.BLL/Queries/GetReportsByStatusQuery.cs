using MediatR;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Queries;

public sealed record GetReportsByStatusQuery(ReportStatus Status) : IRequest<IReadOnlyList<Report>>;
