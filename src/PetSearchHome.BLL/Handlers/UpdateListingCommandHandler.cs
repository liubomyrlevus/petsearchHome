using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Handlers;

public class UpdateListingCommandHandler : IRequestHandler<UpdateListingCommand>
{
    private readonly IListingRepository _listingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateListingCommandHandler(IListingRepository listingRepository, IUnitOfWork unitOfWork)
    {
        _listingRepository = listingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateListingCommand request, CancellationToken cancellationToken)
    {
        var listingToUpdate = await _listingRepository.GetByIdAsync(request.Id, cancellationToken);

        if (listingToUpdate == null)
        {
            throw new Exception("Listing not found.");
        }

        if (listingToUpdate.UserId != request.UserId)
        {
            throw new Exception("User is not authorized to edit this listing.");
        }

        listingToUpdate.AnimalType = request.AnimalType;
        listingToUpdate.Breed = request.Breed;
        listingToUpdate.AgeMonths = request.AgeMonths;
        listingToUpdate.Sex = request.Sex;
        listingToUpdate.Size = request.Size;
        listingToUpdate.Color = request.Color;
        listingToUpdate.City = request.City;
        listingToUpdate.District = request.District;
        listingToUpdate.Description = request.Description;
        listingToUpdate.SpecialNeeds = request.SpecialNeeds;
        listingToUpdate.Status = ListingStatus.pending;
        listingToUpdate.UpdatedAt = DateTime.UtcNow;

        if (request.HealthInfo != null)
        {
            listingToUpdate.HealthInfo ??= new HealthInfo();
            listingToUpdate.HealthInfo.Vaccinations = request.HealthInfo.Vaccinations;
            listingToUpdate.HealthInfo.Sterilized = request.HealthInfo.Sterilized;
            listingToUpdate.HealthInfo.ChronicDiseases = request.HealthInfo.ChronicDiseases;
            listingToUpdate.HealthInfo.TreatmentHistory = request.HealthInfo.TreatmentHistory;
        }

        listingToUpdate.Photos.Clear();
        listingToUpdate.Photos = request.PhotoUrls.Select((url, index) => new ListingPhoto
        {
            Url = url,
            IsPrimary = index == 0
        }).ToList();

        await _listingRepository.UpdateAsync(listingToUpdate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

