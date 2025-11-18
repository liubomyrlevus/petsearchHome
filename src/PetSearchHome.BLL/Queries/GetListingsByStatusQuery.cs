using MediatR;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Queries;

public class GetListingsByStatusQuery : IRequest<IReadOnlyList<ListingModerationDto>>
{
    public ListingStatus Status { get; set; }
}
