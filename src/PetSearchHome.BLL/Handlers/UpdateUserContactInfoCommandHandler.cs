using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Handlers;

public class UpdateUserContactInfoCommandHandler : IRequestHandler<UpdateUserContactInfoCommand, UserProfileDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserContactInfoCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserProfileDto> Handle(UpdateUserContactInfoCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        if (user.IndividualProfile != null && request.Individual != null)
        {
            user.IndividualProfile.Phone = request.Individual.Phone;
            user.IndividualProfile.City = request.Individual.City;
            user.IndividualProfile.District = request.Individual.District;
            user.IndividualProfile.AdditionalInfo = request.Individual.AdditionalInfo;
        }

        if (user.ShelterProfile != null && request.Shelter != null)
        {
            user.ShelterProfile.ContactPerson = request.Shelter.ContactPerson;
            user.ShelterProfile.Phone = request.Shelter.Phone;
            user.ShelterProfile.Address = request.Shelter.Address;
            user.ShelterProfile.Description = request.Shelter.Description;
        }

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(user);
    }

    private static UserProfileDto MapToDto(RegisteredUser user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            UserType = user.UserType,
            IndividualProfile = user.IndividualProfile == null
                ? null
                : new IndividualProfileDto
                {
                    FirstName = user.IndividualProfile.FirstName,
                    LastName = user.IndividualProfile.LastName,
                    Phone = user.IndividualProfile.Phone,
                    City = user.IndividualProfile.City,
                    District = user.IndividualProfile.District,
                    AdditionalInfo = user.IndividualProfile.AdditionalInfo,
                    PhotoUrl = user.IndividualProfile.PhotoUrl
                },
            ShelterProfile = user.ShelterProfile == null
                ? null
                : new ShelterProfileDto
                {
                    Name = user.ShelterProfile.Name,
                    ContactPerson = user.ShelterProfile.ContactPerson,
                    Phone = user.ShelterProfile.Phone,
                    Address = user.ShelterProfile.Address,
                    Description = user.ShelterProfile.Description,
                    LogoUrl = user.ShelterProfile.LogoUrl
                }
        };
    }
}
