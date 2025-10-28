using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetReviewsForUserQuery(Guid ReviewedUserId) : IRequest<IReadOnlyList<Review>>;
