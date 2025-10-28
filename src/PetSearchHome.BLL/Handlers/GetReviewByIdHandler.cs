using MediatR;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Queries;

namespace PetSearchHome.BLL.Handlers;

public sealed class GetReviewByIdHandler : IRequestHandler<GetReviewByIdQuery, Review?>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewByIdHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Review?> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty) return null;

        var r = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        return r;
    }
}
