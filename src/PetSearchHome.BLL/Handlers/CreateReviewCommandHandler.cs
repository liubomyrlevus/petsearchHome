using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.DAL.Domain.Entities;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.DAL.Domain.Enums;
namespace PetSearchHome.BLL.Handlers;
public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateReviewCommandHandler(IReviewRepository reviewRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    { _reviewRepository = reviewRepository; _userRepository = userRepository; _unitOfWork = unitOfWork; }
    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.ReviewerId == request.ReviewedId) { throw new Exception("Users cannot review themselves."); }
        var review = new Review
        {
            ReviewerId = request.ReviewerId,
            ReviewedId = request.ReviewedId,
            Rating = request.Rating,
            Comment = request.Comment,
        };
        await _reviewRepository.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var reviewer = await _userRepository.GetByIdAsync(request.ReviewerId, cancellationToken);
        if (reviewer == null) { throw new Exception("Reviewer not found."); }
        return new ReviewDto
        {
            Id = review.Id,
            ReviewerId = review.ReviewerId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            ReviewerName = reviewer.UserType == UserType.shelter ? reviewer.ShelterProfile!.Name : $"{reviewer.IndividualProfile!.FirstName} {reviewer.IndividualProfile!.LastName}",
            ReviewerAvatarUrl = reviewer.UserType == UserType.shelter ? reviewer.ShelterProfile!.LogoUrl : reviewer.IndividualProfile!.PhotoUrl
        };
    }
}