using MediatR;
namespace PetSearchHome.BLL.Commands;
public class InvalidateSessionCommand : IRequest
{
    public Guid SessionId { get; set; } 
}