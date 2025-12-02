using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.DAL.Contracts.Persistence;
namespace PetSearchHome.BLL.Handlers;
public class InvalidateSessionCommandHandler : IRequestHandler<InvalidateSessionCommand>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    public InvalidateSessionCommandHandler(ISessionRepository sessionRepository, IUnitOfWork unitOfWork)
    { _sessionRepository = sessionRepository; _unitOfWork = unitOfWork; }
    public async Task<Unit> Handle(InvalidateSessionCommand request, CancellationToken cancellationToken)
    {
        await _sessionRepository.InvalidateAsync(request.SessionId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}