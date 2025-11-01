using MediatR;

namespace PetSearchHome.BLL.Commands;

// Команда для завершення сесії користувача
public class InvalidateSessionCommand : IRequest
{
    public string SessionToken { get; set; } = string.Empty;
}