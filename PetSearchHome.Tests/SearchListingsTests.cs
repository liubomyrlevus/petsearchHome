using Moq;
using Xunit;
using PetSearchHome.BLL.Handlers;
using PetSearchHome.BLL.Queries;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.DAL.Domain.Entities;
using PetSearchHome.DAL.Domain.Enums;
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
    public async Task Handle_Should_Respect_Status_From_Query_For_Public_Search()
    {
        var query = new SearchListingsQuery
        {
            City = "Lviv",
            AnimalType = AnimalType.cat,
            Status = ListingStatus.pending
        };

        var fakeListings = new List<Listing>
        {
            new Listing { Id = 1, Breed = "Persian", City = "Lviv", AnimalType = AnimalType.cat, Status = ListingStatus.pending }
        };

        _listingRepoMock.Setup(repo => repo.SearchAsync(
            query.SearchQuery,
            query.AnimalType,
            query.City,
            query.Status,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeListings);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(fakeListings[0].Id, result[0].Id);

        _listingRepoMock.Verify(repo => repo.SearchAsync(
            query.SearchQuery,
            query.AnimalType,
            query.City,
            query.Status,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Filter_User_Listings_By_Status()
    {
        var query = new SearchListingsQuery
        {
            UserId = 42,
            Status = ListingStatus.archived
        };

        var userListings = new List<Listing>
        {
            new Listing { Id = 1, UserId = 42, Status = ListingStatus.archived },
            new Listing { Id = 2, UserId = 42, Status = ListingStatus.active }
        };

        _listingRepoMock.Setup(repo => repo.GetByOwnerAsync(query.UserId.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userListings);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);

        _listingRepoMock.Verify(repo => repo.GetByOwnerAsync(query.UserId.Value, It.IsAny<CancellationToken>()), Times.Once);
        _listingRepoMock.Verify(repo => repo.SearchAsync(
            It.IsAny<string?>(),
            It.IsAny<AnimalType?>(),
            It.IsAny<string?>(),
            It.IsAny<ListingStatus?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}
