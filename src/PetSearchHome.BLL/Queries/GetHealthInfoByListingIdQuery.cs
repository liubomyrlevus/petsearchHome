using MediatR;
using PetSearchHome.BLL.Domain.Entities;

namespace PetSearchHome.BLL.Queries;

public sealed record GetHealthInfoByListingIdQuery(int ListingId) : IRequest<HealthInfo?>;
