using Moq;
using Xunit;
using PetSearchHome.BLL.Handlers;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Queries;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;

namespace PetSearchHome.Tests;

public class FavoritesTests
{
    private readonly Mock<IFavoriteRepository> _favRepoMock;
    private readonly Mock<IListingRepository> _listingRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;

    public FavoritesTests()
    {
        _favRepoMock = new Mock<IFavoriteRepository>();
        _listingRepoMock = new Mock<IListingRepository>();
        _uowMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task AddFavorite_Should_Add_If_Not_Exists()
    {
        var handler = new AddFavoriteCommandHandler(_favRepoMock.Object, _uowMock.Object);
        var command = new AddFavoriteCommand { UserId = 1, ListingId = 10 };

        _favRepoMock.Setup(r => r.ExistsAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); 


        await handler.Handle(command, CancellationToken.None);


        _favRepoMock.Verify(r => r.AddAsync(It.IsAny<Favorite>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddFavorite_Should_Do_Nothing_If_Already_Exists()
    {
        var handler = new AddFavoriteCommandHandler(_favRepoMock.Object, _uowMock.Object);
        var command = new AddFavoriteCommand { UserId = 1, ListingId = 10 };

        _favRepoMock.Setup(r => r.ExistsAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await handler.Handle(command, CancellationToken.None);

        _favRepoMock.Verify(r => r.AddAsync(It.IsAny<Favorite>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoveFavorite_Should_Call_Remove_And_Save()
    {
        var handler = new RemoveFavoriteCommandHandler(_favRepoMock.Object, _uowMock.Object);
        var command = new RemoveFavoriteCommand { UserId = 1, ListingId = 10 };

        await handler.Handle(command, CancellationToken.None);

        _favRepoMock.Verify(r => r.RemoveAsync(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserFavorites_Should_Return_Mapped_Dtos()
    {
        var handler = new GetUserFavoritesQueryHandler(_favRepoMock.Object, _listingRepoMock.Object);
        var query = new GetUserFavoritesQuery { UserId = 1 };

        var favorites = new List<Favorite> { new Favorite { UserId = 1, ListingId = 10 } };
        var listings = new List<Listing>
        {
            new Listing { Id = 10, Breed = "Labrador", AnimalType = AnimalType.dog, Photos = new List<ListingPhoto>() }
        };

        _favRepoMock.Setup(r => r.GetByUserAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(favorites);
        _listingRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(listings);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Labrador", result[0].Breed);
    }
}