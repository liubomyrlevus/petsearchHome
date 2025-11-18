using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.BLL.Services.Authentication;
using PetSearchHome.BLL.Domain.Entities;
using System.Diagnostics;

namespace PetSearchHome.BLL.Handlers;

public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ISessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginQueryHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, ISessionRepository sessionRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResultDto> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        Debug.WriteLine("--- LOGIN ATTEMPT ---");
        Debug.WriteLine($"Attempting login for: {request.Email}");

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
        {
            Debug.WriteLine("Result: FAILED. User not found in database.");
            Debug.WriteLine("-------------------------");
            throw new Exception("Invalid email or password.");
        }

        Debug.WriteLine($"User found. DB Hash: {user.PasswordHash}");
        bool isPasswordCorrect = _passwordHasher.Verify(request.Password, user.PasswordHash);
        Debug.WriteLine($"Password verification result: {isPasswordCorrect}");
        Debug.WriteLine("-------------------------");

        if (!user.IsActive || !isPasswordCorrect)
        {
            throw new Exception("Invalid email or password.");
        }

        var session = new Session
        {
            UserId = user.Id,
            SessionToken = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        await _sessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user);

        // --- ОСЬ ВИПРАВЛЕНИЙ БЛОК ---
        var profileDto = new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            UserType = user.UserType,

            // 1. ВИПРАВЛЕНО: 'new IndividualProfileDto'
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

            // 2. ВИПРАВЛЕНО: 'new ShelterProfileDto'
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
        // --- КІНЕЦЬ ВИПРАВЛЕНОГО БЛОКУ ---

        return new LoginResultDto { User = profileDto, Token = token };
    }
}