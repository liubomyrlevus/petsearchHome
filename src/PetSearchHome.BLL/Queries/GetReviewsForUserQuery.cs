using MediatR;
using PetSearchHome.BLL.DTOs;
namespace PetSearchHome.BLL.Queries;
public class GetReviewsForUserQuery : IRequest<IReadOnlyList<ReviewDto>>
{
    public int ReviewedUserId { get; set; } 
}