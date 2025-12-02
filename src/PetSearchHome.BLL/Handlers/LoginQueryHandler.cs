using MediatR;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.BLL.Services.Authentication;
using PetSearchHome.DAL.Domain.Entities;

namespace PetSearchHome.BLL.Handlers;

public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ISessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginQueryHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ISessionRepository sessionRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResultDto> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        // 🔍 Перевірка email/пароля
        if (user == null || !user.IsActive ||
            !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return new LoginResultDto
            {
                IsSuccess = false,
                Error = "Невірний email або пароль."
            };
        }

        // 🔐 Створення сесії
        var session = new Session
        {
            UserId = user.Id,
            SessionToken = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        await _sessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 🔑 Генерація JWT
        var token = _jwtTokenGenerator.GenerateToken(user);

        // 🧍 Профіль користувача
        var profileDto = new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            UserType = user.UserType,
            IndividualProfile = user.IndividualProfile != null ? new IndividualProfileDto
            {
                FirstName = user.IndividualProfile.FirstName,
                LastName = user.IndividualProfile.LastName,
                Phone = user.IndividualProfile.Phone,
                City = user.IndividualProfile.City,
                District = user.IndividualProfile.District,
                AdditionalInfo = user.IndividualProfile.AdditionalInfo,
                PhotoUrl = user.IndividualProfile.PhotoUrl
            } : null,
            ShelterProfile = user.ShelterProfile != null ? new ShelterProfileDto
            {
                Name = user.ShelterProfile.Name,
                ContactPerson = user.ShelterProfile.ContactPerson,
                Phone = user.ShelterProfile.Phone,
                Address = user.ShelterProfile.Address,
                Description = user.ShelterProfile.Description,
                LogoUrl = user.ShelterProfile.LogoUrl
            } : null
        };

        // 🎉 Успішний логін
        return new LoginResultDto
        {
            IsSuccess = true,
            Token = token,
            User = profileDto
        };
    }
}
