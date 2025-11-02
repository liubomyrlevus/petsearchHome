using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Domain.Enums; // Необхідно для UserType

namespace PetSearchHome.BLL.Handlers;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewCommandHandler(IReviewRepository reviewRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        // Валідація
        if (request.ReviewerId == request.ReviewedId)
        {
            throw new Exception("Users cannot review themselves.");
        }
        if (request.Rating < 1 || request.Rating > 5)
        {
            throw new Exception("Rating must be between 1 and 5.");
        }

        // Створюємо
        var review = new Review
        {
            ReviewerId = request.ReviewerId,
            ReviewedId = request.ReviewedId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow,
            IsModerated = false 
        };

        await _reviewRepository.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var reviewer = await _userRepository.GetByIdAsync(request.ReviewerId, cancellationToken);
        if (reviewer == null)
        {
            throw new Exception("Reviewer not found."); 
        }

        return new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            ReviewerName = reviewer.UserType == UserType.Shelter
                ? reviewer.ShelterProfile.Name
                : $"{reviewer.IndividualProfile.FirstName} {reviewer.IndividualProfile.LastName}",
            ReviewerAvatarUrl = reviewer.UserType == UserType.Shelter
                ? reviewer.ShelterProfile.LogoUrl
                : reviewer.IndividualProfile.PhotoUrl
        };
    }
}