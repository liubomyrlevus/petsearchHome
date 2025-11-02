using MediatR;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Queries;

//список усіх відгуків для вказаного профілю

public class GetReviewsForUserQuery : IRequest<IReadOnlyList<ReviewDto>>
{
    public Guid ReviewedUserId { get; set; }
}