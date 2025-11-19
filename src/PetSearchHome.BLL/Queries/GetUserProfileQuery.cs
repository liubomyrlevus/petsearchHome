using MediatR;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Queries;

public class GetUserProfileQuery : IRequest<UserProfileDto>
{
    public int UserId { get; set; }
}
