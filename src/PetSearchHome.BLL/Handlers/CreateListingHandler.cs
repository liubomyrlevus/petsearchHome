using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Handlers;

public sealed class CreateListingHandler : IRequestHandler<CreateListingCommand, int>
{
    private readonly IListingRepository _listingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateListingHandler(IListingRepository listingRepository, IUnitOfWork unitOfWork)
    {
        _listingRepository = listingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateListingCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty) return -1;

        var listing = new Listing
        {
            UserId = request.UserId,
            AnimalType = request.AnimalType,
            Breed = request.Breed ?? string.Empty,
            AgeMonths = request.AgeMonths,
            Sex = request.Sex,
            Size = request.Size,
            Color = request.Color ?? string.Empty,
            City = request.City ?? string.Empty,
            District = request.District ?? string.Empty,
            Description = request.Description ?? string.Empty,
            SpecialNeeds = request.SpecialNeeds,
            HealthInfo = request.HealthInfo,
            Photos = request.Photos ?? new List<ListingPhoto>(),
            CreatedAt = DateTime.UtcNow,
            Status = PetSearchHome.BLL.Domain.Enums.ListingStatus.Pending
        };

        var id = await _listingRepository.AddAsync(listing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return id;
    }
}
