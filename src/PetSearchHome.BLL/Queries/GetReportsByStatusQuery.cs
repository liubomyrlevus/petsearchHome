using MediatR;
using PetSearchHome.DAL.Domain.Enums;
using PetSearchHome.BLL.DTOs;
namespace PetSearchHome.BLL.Queries;
public class GetReportsByStatusQuery : IRequest<IReadOnlyList<ReportDto>>
{
    public ReportStatus Status { get; set; }
}
//фільтрація для адмінів
