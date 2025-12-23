using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.BLL.Domain.Enums;
namespace PetSearchHome.BLL.Handlers;
public class GetListingDetailsQueryHandler : IRequestHandler<GetListingDetailsQuery, ListingDetailsDto>
{
    private readonly IListingRepository _listingRepository;
    private readonly IUserRepository _userRepository;
    public GetListingDetailsQueryHandler(IListingRepository listingRepository, IUserRepository userRepository)
    { _listingRepository = listingRepository; _userRepository = userRepository; }
    public async Task<ListingDetailsDto> Handle(GetListingDetailsQuery request, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(request.Id, cancellationToken);
        if (listing == null || listing.Status != ListingStatus.active) { throw new Exception("Listing not found or not active."); }
        var owner = await _userRepository.GetByIdAsync(listing.UserId, cancellationToken);
        if (owner == null) { throw new Exception("Owner not found."); }
        var dto = new ListingDetailsDto
        {
            Id = listing.Id,
            UserId = listing.UserId,
            OwnerName = owner.UserType == UserType.shelter ? owner.ShelterProfile!.Name : $"{owner.IndividualProfile!.FirstName} {owner.IndividualProfile!.LastName}",
            AnimalType = listing.AnimalType,
            Breed = listing.Breed,
            AgeMonths = listing.AgeMonths,
            Sex = listing.Sex,
            Size = listing.Size,
            Color = listing.Color,
            City = listing.City,
            District = listing.District,
            Description = listing.Description,
            SpecialNeeds = listing.SpecialNeeds,
            ViewsCount = listing.ViewsCount,
            CreatedAt = listing.CreatedAt,
            PhotoUrls = listing.Photos.Select(p => p.Url).ToList(),
            HealthInfo = listing.HealthInfo != null ? new HealthInfoDto
            {
                Vaccinations = listing.HealthInfo.Vaccinations,
                Sterilized = listing.HealthInfo.Sterilized,
                ChronicDiseases = listing.HealthInfo.ChronicDiseases,
                TreatmentHistory = listing.HealthInfo.TreatmentHistory
            } : null
        };

        return dto;
    }
}