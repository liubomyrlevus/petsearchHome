using MediatR;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.BLL.Commands;

public sealed record CreateListingCommand(
    Guid UserId,
    AnimalType AnimalType,
    string Breed,
    int AgeMonths,
    AnimalSex Sex,
    AnimalSize Size,
    string Color,
    string City,
    string District,
    string Description,
    string? SpecialNeeds,
    HealthInfo? HealthInfo,
    List<ListingPhoto>? Photos) : IRequest<int>;
