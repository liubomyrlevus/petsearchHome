using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
namespace PetSearchHome.BLL.Handlers;
public class UpdateListingCommandHandler : IRequestHandler<UpdateListingCommand>
{
    private readonly IListingRepository _listingRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateListingCommandHandler(IListingRepository listingRepository, IUnitOfWork unitOfWork)
    { _listingRepository = listingRepository; _unitOfWork = unitOfWork; }
    public async Task<Unit> Handle(UpdateListingCommand request, CancellationToken cancellationToken)
    {
        var listingToUpdate = await _listingRepository.GetByIdAsync(request.Id, cancellationToken);
        if (listingToUpdate == null) { throw new Exception("Listing not found."); }
        if (listingToUpdate.UserId != request.UserId) { throw new Exception("User is not authorized to edit this listing."); }

        await _listingRepository.UpdateAsync(listingToUpdate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}