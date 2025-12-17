using MediatR;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Queries;

public class GetAllUsersQuery : IRequest<IReadOnlyList<UserSummaryDto>>
{
}
