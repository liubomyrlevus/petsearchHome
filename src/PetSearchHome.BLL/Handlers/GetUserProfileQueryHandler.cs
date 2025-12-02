using MediatR;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.DAL.Domain.Entities;

namespace PetSearchHome.BLL.Handlers;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserProfileQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

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
