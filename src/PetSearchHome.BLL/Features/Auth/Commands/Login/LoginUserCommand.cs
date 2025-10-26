using MediatR;
using PetSearchHome.BLL.Features.Auth.DTOs;

namespace PetSearchHome.BLL.Features.Auth.Commands.Login;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<LoginResultDto>;
