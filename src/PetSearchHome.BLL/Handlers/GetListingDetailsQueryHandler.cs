using MediatR;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.DAL.Domain.Enums;
using Microsoft.Extensions.Logging;
namespace PetSearchHome.BLL.Handlers;
public class GetListingDetailsQueryHandler : IRequestHandler<GetListingDetailsQuery, ListingDetailsDto>
{
    private readonly IListingRepository _listingRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetListingDetailsQueryHandler> _logger;
    public GetListingDetailsQueryHandler(IListingRepository listingRepository, IUserRepository userRepository, ILogger<GetListingDetailsQueryHandler> logger)
    { _listingRepository = listingRepository; _userRepository = userRepository; _logger = logger; }
    public async Task<ListingDetailsDto> Handle(GetListingDetailsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching listing details for Id {ListingId}", request.Id);
        var listing = await _listingRepository.GetByIdAsync(request.Id, cancellationToken);
        if (listing == null || listing.Status == ListingStatus.archived || listing.Status == ListingStatus.rejected) { _logger.LogWarning("Listing {ListingId} not found or inactive", request.Id); throw new Exception("Listing not found or not active."); }
        var owner = await _userRepository.GetByIdAsync(listing.UserId, cancellationToken);
        if (owner == null) { _logger.LogError("Owner {OwnerId} not found for Listing {ListingId}", listing.UserId, request.Id); throw new Exception("Owner not found."); }
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

        _logger.LogInformation("Listing details prepared for Id {ListingId}", request.Id);
        return dto;
    }
}