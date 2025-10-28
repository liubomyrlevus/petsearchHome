using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Queries;

namespace PetSearchHome.BLL.Handlers;

public sealed class GetSessionByTokenHandler : IRequestHandler<GetSessionByTokenQuery, Session?>
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionByTokenHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Session?> Handle(GetSessionByTokenQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token)) return null;

        var session = await _sessionRepository.GetByTokenAsync(request.Token, cancellationToken);
        return session;
    }
}
