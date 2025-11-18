using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
namespace PetSearchHome.BLL.Handlers;
public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IUnitOfWork _unitOfWork;
    public AddFavoriteCommandHandler(IFavoriteRepository favoriteRepository, IUnitOfWork unitOfWork)
    {
        _favoriteRepository = favoriteRepository; _unitOfWork = unitOfWork;
    }
    public async Task<Unit> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
    {
        var alreadyExists = await _favoriteRepository.ExistsAsync(request.UserId, request.ListingId, cancellationToken);
        if (alreadyExists) { return Unit.Value; }
        var favorite = new Favorite
        {
            UserId = request.UserId,
            ListingId = request.ListingId
        };
        await _favoriteRepository.AddAsync(favorite, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}