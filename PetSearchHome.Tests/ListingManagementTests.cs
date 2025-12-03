using Moq;
using Xunit;
using PetSearchHome.BLL.Handlers;
using PetSearchHome.BLL.Commands;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.DAL.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace PetSearchHome.Tests;

public class ListingManagementTests
{
    private readonly Mock<IListingRepository> _listingRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;

    public ListingManagementTests()
    {
        _listingRepoMock = new Mock<IListingRepository>();
        _uowMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task Delete_Should_Throw_If_Not_Owner()
    {
        var handler = new DeleteListingCommandHandler(_listingRepoMock.Object, _uowMock.Object);

        var command = new DeleteListingCommand { ListingId = 10, UserId = 999 };

        var listing = new Listing { Id = 10, UserId = 1 };


        _listingRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(listing);

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            handler.Handle(command, CancellationToken.None));

        Assert.Equal("User is not authorized to delete this listing.", exception.Message);

        _listingRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Delete_Should_Succeed_If_Owner()
    {
        var handler = new DeleteListingCommandHandler(_listingRepoMock.Object, _uowMock.Object);

        var command = new DeleteListingCommand { ListingId = 10, UserId = 1 };
        var listing = new Listing { Id = 10, UserId = 1 };

        _listingRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(listing);

        await handler.Handle(command, CancellationToken.None);

        _listingRepoMock.Verify(r => r.DeleteAsync(10, It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}