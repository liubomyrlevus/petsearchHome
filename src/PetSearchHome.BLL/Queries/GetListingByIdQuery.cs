using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetListingByIdQuery(int Id) : IRequest<Listing?>;
