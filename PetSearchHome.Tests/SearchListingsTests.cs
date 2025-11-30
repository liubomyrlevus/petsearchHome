using Moq;
using Xunit;
using PetSearchHome.BLL.Handlers;
using PetSearchHome.BLL.Queries;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PetSearchHome.Tests;

public class SearchListingsTests
{
    private readonly Mock<IListingRepository> _listingRepoMock;
    private readonly SearchListingsQueryHandler _handler;

    public SearchListingsTests()
    {
        _listingRepoMock = new Mock<IListingRepository>();
        _handler = new SearchListingsQueryHandler(_listingRepoMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Call_Repository_With_Active_Status()
    {
        var query = new SearchListingsQuery
        {
            City = "Lviv",
            AnimalType = AnimalType.cat
        };

        var fakeListings = new List<Listing>
        {
            new Listing { Id = 1, Breed = "Persian", City = "Lviv", AnimalType = AnimalType.cat }
        };

        _listingRepoMock.Setup(repo => repo.SearchAsync(
            It.IsAny<string?>(),        // searchQuery
            It.IsAny<AnimalType?>(),    // animalType
            It.IsAny<string?>(),        // city
            ListingStatus.active,       // status (це те, що ми перевіряємо!)
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeListings);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotEmpty(result);
        Assert.Equal(1, result[0].Id);

        _listingRepoMock.Verify(repo => repo.SearchAsync(
            query.SearchQuery,
            query.AnimalType,
            query.City,
            ListingStatus.active, 
            It.IsAny<CancellationToken>()), Times.Once);
    }
}