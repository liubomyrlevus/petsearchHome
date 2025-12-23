using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Services.Authentication;
namespace PetSearchHome.BLL.Handlers;
public class RegisterIndividualCommandHandler : IRequestHandler<RegisterIndividualCommand, LoginResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    public RegisterIndividualCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository; _passwordHasher = passwordHasher; _jwtTokenGenerator = jwtTokenGenerator; _unitOfWork = unitOfWork;
    }
    public async Task<LoginResultDto> Handle(RegisterIndividualCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByEmailAsync(request.Email, cancellationToken) != null)
        { throw new Exception("Email is already taken."); }
        var user = new RegisteredUser
        {
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            UserType = UserType.individual,
            IndividualProfile = new IndividualProfile
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                City = request.City,
                District = request.District
            }
        };
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var token = _jwtTokenGenerator.GenerateToken(user);
        var profileDto = new UserProfileDto { /* ... мапінг ... */ };
        return new LoginResultDto { User = profileDto, Token = token };
    }
}