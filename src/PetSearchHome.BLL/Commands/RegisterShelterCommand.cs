using MediatR;
using PetSearchHome.BLL.DTOs;
namespace PetSearchHome.BLL.Commands;
public class RegisterShelterCommand : IRequest<LoginResultDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}