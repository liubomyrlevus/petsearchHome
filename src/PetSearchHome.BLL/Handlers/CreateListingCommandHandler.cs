using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;
namespace PetSearchHome.BLL.Handlers;
public class CreateListingCommandHandler : IRequestHandler<CreateListingCommand, int>
{
    private readonly IListingRepository _listingRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateListingCommandHandler(IListingRepository listingRepository, IUnitOfWork unitOfWork)
    { _listingRepository = listingRepository; _unitOfWork = unitOfWork; }
    public async Task<int> Handle(CreateListingCommand request, CancellationToken cancellationToken)
    {
        var listing = new Listing
        {
            UserId = request.UserId,
            AnimalType = request.AnimalType,
            Breed = request.Breed,
            AgeMonths = request.AgeMonths,
            Sex = request.Sex,
            Size = request.Size,
            Color = request.Color,
            City = request.City,
            District = request.District,
            Description = request.Description,
            SpecialNeeds = request.SpecialNeeds,
            // Нові оголошення одразу робимо активними,
            // щоб вони з'являлися на головній сторінці.
            Status = ListingStatus.active,
            Photos = request.PhotoUrls.Select((url, index) => new ListingPhoto
            {
                Url = url,
                IsPrimary = (index == 0)
            }).ToList()
        };
        if (request.HealthInfo != null)
        {
            listing.HealthInfo = new HealthInfo
            {
                Vaccinations = request.HealthInfo.Vaccinations,
                Sterilized = request.HealthInfo.Sterilized,
                ChronicDiseases = request.HealthInfo.ChronicDiseases,
                TreatmentHistory = request.HealthInfo.TreatmentHistory
            };
        }
        var listingId = await _listingRepository.AddAsync(listing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return listingId;
    }
}
