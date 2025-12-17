using MediatR;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.DAL.Contracts.Persistence;
using System.Linq;

namespace PetSearchHome.BLL.Handlers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IReadOnlyList<UserSummaryDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserSummaryDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        return users
            .Select(u => new UserSummaryDto
            {
                Id = u.Id,
                Email = u.Email,
                UserType = u.UserType.ToString(),
                IsActive = u.IsActive,
                IsAdmin = u.IsAdmin,
                CreatedAt = u.CreatedAt
            })
            .ToList();
    }
}
