using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Features.Auth.DTOs;
using PetSearchHome.BLL.Services.Authentication;

namespace PetSearchHome.BLL.Features.Auth.Commands.Login;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginResultDto
            {
                IsSuccess = false,
                Error = "Email та пароль обов'язкові."
            };
        }

        var user = await _userRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return new LoginResultDto
            {
                IsSuccess = false,
                Error = "Неправильний email або password."
            };
        }

        if (!user.IsActive)
        {
            return new LoginResultDto
            {
                IsSuccess = false,
                Error = "Ваш акаунт деактивовано. Зверніться у підтримку."
            };
        }

        user.LastLogin = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new LoginResultDto
        {
            IsSuccess = true,
            Token = token
        };
    }
}
