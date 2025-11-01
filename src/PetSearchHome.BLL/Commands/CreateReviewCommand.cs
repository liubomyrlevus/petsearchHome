using MediatR;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Commands;

public class CreateReviewCommand : IRequest<ReviewDto>
{
    public Guid ReviewerId { get; set; }  // Хто залишає відгук
    public Guid ReviewedId { get; set; }  // Про кого залишають відгук
    public int Rating { get; set; }       // Оцінка (наприклад, 1-5)
    public string? Comment { get; set; }
}