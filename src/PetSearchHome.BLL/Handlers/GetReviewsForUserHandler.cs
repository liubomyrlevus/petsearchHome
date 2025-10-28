using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Queries;

namespace PetSearchHome.BLL.Handlers;

public sealed class GetReviewsForUserHandler : IRequestHandler<GetReviewsForUserQuery, IReadOnlyList<Review>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewsForUserHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IReadOnlyList<Review>> Handle(GetReviewsForUserQuery request, CancellationToken cancellationToken)
    {
        if (request.ReviewedUserId == Guid.Empty) return Array.Empty<Review>();

        var reviews = await _reviewRepository.GetForUserAsync(request.ReviewedUserId, cancellationToken);
        return reviews;
    }
}
