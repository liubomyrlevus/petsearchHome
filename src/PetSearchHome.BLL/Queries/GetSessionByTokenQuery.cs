using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetSessionByTokenQuery(string Token) : IRequest<Session?>;
