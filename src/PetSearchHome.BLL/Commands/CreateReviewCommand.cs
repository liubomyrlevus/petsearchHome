using MediatR;
using PetSearchHome.BLL.DTOs;
namespace PetSearchHome.BLL.Commands;
public class CreateReviewCommand : IRequest<ReviewDto>
{
    public int ReviewerId { get; set; } 
    public int ReviewedId { get; set; } 
    public int Rating { get; set; }
    public string? Comment { get; set; }
}