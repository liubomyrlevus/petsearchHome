using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetReportByIdQuery(Guid Id) : IRequest<Report?>;
