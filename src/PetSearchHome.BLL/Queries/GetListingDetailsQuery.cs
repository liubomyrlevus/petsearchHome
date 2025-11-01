using MediatR;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Queries;
//детальна інформація про оголошення

public class GetListingDetailsQuery : IRequest<ListingDetailsDto>
{
    public int Id { get; set; }
}