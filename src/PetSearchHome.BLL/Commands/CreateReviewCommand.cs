using MediatR;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Commands;

public class CreateReviewCommand : IRequest<ReviewDto>
{
    public int ReviewerId { get; set; }   // Хто залишає відгук
    public int ReviewedId { get; set; }   // Про кого залишають відгук
    public int Rating { get; set; }        // Оцінка (наприклад, 1-5)
    public string? Comment { get; set; }
}