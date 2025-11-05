using MediatR;
using PetSearchHome.BLL.DTOs;
namespace PetSearchHome.BLL.Queries;
public class LoginQuery : IRequest<LoginResultDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}