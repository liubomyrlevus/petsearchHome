using MediatR;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.DAL.Domain.Enums;
namespace PetSearchHome.BLL.Handlers;
public class GetReviewsForUserQueryHandler : IRequestHandler<GetReviewsForUserQuery, IReadOnlyList<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    public GetReviewsForUserQueryHandler(IReviewRepository reviewRepository, IUserRepository userRepository)
    { _reviewRepository = reviewRepository; _userRepository = userRepository; }
    public async Task<IReadOnlyList<ReviewDto>> Handle(GetReviewsForUserQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetForUserAsync(request.ReviewedUserId, cancellationToken);
        var result = new List<ReviewDto>();
        var reviewerIds = reviews.Select(r => r.ReviewerId).Distinct().ToList();
        var reviewers = (await Task.WhenAll(reviewerIds.Select(id => _userRepository.GetByIdAsync(id, cancellationToken))))
            .Where(u => u != null).ToDictionary(u => u!.Id);
        foreach (var review in reviews)
        {
            if (reviewers.TryGetValue(review.ReviewerId, out var reviewer))
            {
                result.Add(new ReviewDto
                {
                    Id = review.Id,
                    ReviewerId = review.ReviewerId,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt,
                    ReviewerName = reviewer.UserType == UserType.shelter ? reviewer.ShelterProfile!.Name : $"{reviewer.IndividualProfile!.FirstName} {reviewer.IndividualProfile!.LastName}",
                    ReviewerAvatarUrl = reviewer.UserType == UserType.shelter ? reviewer.ShelterProfile!.LogoUrl : reviewer.IndividualProfile!.PhotoUrl
                });
            }
        }
        return result.OrderByDescending(r => r.CreatedAt).ToList();
    }
}