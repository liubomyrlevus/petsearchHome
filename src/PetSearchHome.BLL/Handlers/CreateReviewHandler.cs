using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Handlers;

public sealed class CreateReviewHandler : IRequestHandler<CreateReviewCommand, Review>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewHandler(IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Review> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = new Review
        {
            Id = Guid.NewGuid(),
            ReviewerId = request.ReviewerId,
            ReviewedId = request.ReviewedId,
            Rating = Math.Clamp(request.Rating, 1, 5),
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow,
            IsModerated = false
        };

        await _reviewRepository.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return review;
    }
}
