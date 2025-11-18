using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;

namespace PetSearchHome.BLL.Handlers;

public class ModerateListingCommandHandler : IRequestHandler<ModerateListingCommand>
{
    private readonly IListingRepository _listingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ModerateListingCommandHandler(
        IListingRepository listingRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
 {
        _listingRepository = listingRepository;
        _userRepository = userRepository;
 _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ModerateListingCommand request, CancellationToken cancellationToken)
    {
        // ????????? ???? ??????????
        var moderator = await _userRepository.GetByIdAsync(request.ModeratorId, cancellationToken);
        if (moderator == null || !moderator.IsAdmin)
        {
        throw new Exception("User is not authorized to moderate listings.");
  }

        var listing = await _listingRepository.GetByIdAsync(request.ListingId, cancellationToken);
        if (listing == null)
        {
        throw new Exception("Listing not found.");
        }

        listing.Status = request.NewStatus;
        listing.ModerationComment = request.ModerationComment;
        listing.UpdatedAt = DateTime.UtcNow;

   await _listingRepository.UpdateAsync(listing, cancellationToken);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
