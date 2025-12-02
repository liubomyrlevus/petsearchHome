using MediatR;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;

namespace PetSearchHome.BLL.Handlers;

public class GetListingsByStatusQueryHandler : IRequestHandler<GetListingsByStatusQuery, IReadOnlyList<ListingModerationDto>>
{
    private readonly IListingRepository _listingRepository;
    private readonly IUserRepository _userRepository;

    public GetListingsByStatusQueryHandler(IListingRepository listingRepository, IUserRepository userRepository)
    {
        _listingRepository = listingRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<ListingModerationDto>> Handle(GetListingsByStatusQuery request, CancellationToken cancellationToken)
    {
        var listings = await _listingRepository.SearchAsync(
            searchQuery: null,
            animalType: null,
  city: null,
          status: request.Status,
    cancellationToken);

        var result = new List<ListingModerationDto>();

        foreach (var listing in listings)
    {
       var owner = await _userRepository.GetByIdAsync(listing.UserId, cancellationToken);

            result.Add(new ListingModerationDto
 {
                Id = listing.Id,
            UserId = listing.UserId,
                OwnerEmail = owner?.Email ?? "Unknown",
        AnimalType = listing.AnimalType,
   Breed = listing.Breed,
    AgeMonths = listing.AgeMonths,
           City = listing.City,
          District = listing.District,
          Description = listing.Description,
                Status = listing.Status,
      CreatedAt = listing.CreatedAt,
  UpdatedAt = listing.UpdatedAt,
        ModerationComment = listing.ModerationComment,
          PrimaryPhotoUrl = listing.Photos.FirstOrDefault(p => p.IsPrimary)?.Url
       });
      }

        return result;
    }
}
