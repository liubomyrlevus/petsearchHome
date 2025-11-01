using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Services.Authentication;

namespace PetSearchHome.BLL.Handlers;

public class RegisterShelterCommandHandler : IRequestHandler<RegisterShelterCommand, LoginResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterShelterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResultDto> Handle(RegisterShelterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByEmailAsync(request.Email, cancellationToken) != null)
        {
            throw new Exception("Email is already taken.");
        }

        var user = new RegisteredUser
        {
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            UserType = UserType.Shelter,
            IsActive = true,
            ShelterProfile = new ShelterProfile
            {
                Name = request.Name,
                ContactPerson = request.ContactPerson,
                Phone = request.Phone,
                Address = request.Address
            }
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user);

        var userProfileDto = new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            UserType = user.UserType,
            ShelterProfile = new ShelterProfileDto
            {
                Name = user.ShelterProfile.Name,
                ContactPerson = user.ShelterProfile.ContactPerson,
                Phone = user.ShelterProfile.Phone,
                Address = user.ShelterProfile.Address,
                LogoUrl = user.ShelterProfile.LogoUrl
            }
        };

        return new LoginResultDto { User = userProfileDto, Token = token };
    }
}