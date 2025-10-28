using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Commands;

public sealed record CreateReviewCommand(Guid ReviewerId, Guid ReviewedId, int Rating, string? Comment) : IRequest<Review>;
