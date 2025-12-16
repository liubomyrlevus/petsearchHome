using MediatR;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.BLL.Services.Authentication;
using PetSearchHome.DAL.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace PetSearchHome.BLL.Handlers;

public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ISessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginQueryHandler> _logger;

    public LoginQueryHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ISessionRepository sessionRepository,
        IUnitOfWork unitOfWork,
        ILogger<LoginQueryHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<LoginResultDto> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for {Email}", request.Email);
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        // 🔍 Перевірка email/пароля
        if (user == null || !user.IsActive ||
            !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login for {Email}", request.Email);
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
        _logger.LogInformation("Session created for UserId {UserId}", user.Id);

        // 🔑 Генерація JWT
        var token = _jwtTokenGenerator.GenerateToken(user);

        // 🧍 Профіль користувача
        var profileDto = new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            UserType = user.UserType,
            IsAdmin = user.IsAdmin,
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
        _logger.LogInformation("Successful login for {Email}", request.Email);
        return new LoginResultDto
        {
            IsSuccess = true,
            Token = token,
            User = profileDto
        };
    }
}
