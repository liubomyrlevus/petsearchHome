using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.DAL.Contracts.Persistence;
namespace PetSearchHome.BLL.Handlers;
public class DeleteListingCommandHandler : IRequestHandler<DeleteListingCommand>
{
    private readonly IListingRepository _listingRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteListingCommandHandler(IListingRepository listingRepository, IUnitOfWork unitOfWork)
    { _listingRepository = listingRepository; _unitOfWork = unitOfWork; }
    public async Task<Unit> Handle(DeleteListingCommand request, CancellationToken cancellationToken)
    {
        var listingToDelete = await _listingRepository.GetByIdAsync(request.ListingId, cancellationToken);
        if (listingToDelete == null) { return Unit.Value; }
        if (listingToDelete.UserId != request.UserId) { throw new Exception("User is not authorized to delete this listing."); }
        await _listingRepository.DeleteAsync(request.ListingId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}