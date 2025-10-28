using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetReviewByIdQuery(Guid Id) : IRequest<Review?>;
