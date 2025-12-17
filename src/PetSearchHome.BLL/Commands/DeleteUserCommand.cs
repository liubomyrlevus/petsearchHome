using MediatR;

namespace PetSearchHome.BLL.Commands;

public class DeleteUserCommand : IRequest
{
    public int UserId { get; set; }
}
