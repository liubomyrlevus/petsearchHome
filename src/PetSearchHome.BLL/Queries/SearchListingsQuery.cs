using MediatR;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.DTOs;
namespace PetSearchHome.BLL.Queries;

// Запит на пошук та фільтрацію оголошень для відображення в каталозі.

public class SearchListingsQuery : IRequest<IReadOnlyList<ListingCardDto>>
{
    public string? SearchQuery { get; set; }
    public AnimalType? AnimalType { get; set; }
    public string? City { get; set; }
}