using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;

namespace PetSearchHome.BLL.Handlers;

public class InvalidateSessionCommandHandler : IRequestHandler<InvalidateSessionCommand>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InvalidateSessionCommandHandler(ISessionRepository sessionRepository, IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(InvalidateSessionCommand request, CancellationToken cancellationToken)
    {
        
        var session = await _sessionRepository.GetByTokenAsync(request.SessionToken, cancellationToken);

        
        if (session != null && session.IsValid)
        {
            session.IsValid = false;
            await _sessionRepository.UpdateAsync(session, cancellationToken); 
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}