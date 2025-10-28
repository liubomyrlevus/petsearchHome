using MediatR;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Queries;

public sealed record SearchListingsQuery(
    string? SearchQuery,
    AnimalType? AnimalType,
    string? City,
    ListingStatus? Status) : IRequest<IReadOnlyList<Listing>>;
