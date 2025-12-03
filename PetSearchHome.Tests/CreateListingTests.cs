using Moq;
using Xunit;
using PetSearchHome.BLL.Handlers;
using PetSearchHome.BLL.Commands;
using PetSearchHome.DAL.Contracts.Persistence;
using PetSearchHome.DAL.Domain.Entities;
using PetSearchHome.DAL.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PetSearchHome.Tests;

public class CreateListingTests
{
    private readonly Mock<IListingRepository> _listingRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly CreateListingCommandHandler _handler;

    public CreateListingTests()
    {
        _listingRepoMock = new Mock<IListingRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _handler = new CreateListingCommandHandler(_listingRepoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Listing_And_Save()
    {
        var command = new CreateListingCommand
        {
            UserId = 1,
            AnimalType = AnimalType.dog,
            Description = "Good dog",
            PhotoUrls = new List<string> { "http://photo.url" }
        };

        _listingRepoMock.Setup(r => r.AddAsync(It.IsAny<Listing>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10); 


        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(10, result);

        _listingRepoMock.Verify(r => r.AddAsync(It.Is<Listing>(l =>
            l.Status == ListingStatus.pending &&
            l.UserId == command.UserId &&
            l.Photos.Count == 1),
            It.IsAny<CancellationToken>()), Times.Once);

        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}