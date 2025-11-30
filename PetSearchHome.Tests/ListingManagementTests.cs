using Moq;
using Xunit;
using PetSearchHome.BLL.Handlers;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
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
        // Arrange
        var handler = new DeleteListingCommandHandler(_listingRepoMock.Object, _uowMock.Object);

        // 1. Створюємо команду з ListingId = 10 і UserId = 999 (Зловмисник)
        var command = new DeleteListingCommand { ListingId = 10, UserId = 999 };

        // 2. Створюємо існуюче оголошення з ListingId = 10 і UserId = 1 (Власник)
        var listing = new Listing { Id = 10, UserId = 1 };

        // 3. ВАЖЛИВО: Налаштовуємо Mock так, щоб він повертав наше оголошення,
        // коли питають саме ID 10.
        _listingRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(listing);

        // Act & Assert
        // Ми очікуємо Exception, тому що 999 != 1
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            handler.Handle(command, CancellationToken.None));

        Assert.Equal("User is not authorized to delete this listing.", exception.Message);

        // Переконуємось, що метод видалення НІКОЛИ не викликався
        _listingRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Delete_Should_Succeed_If_Owner()
    {
        // Arrange
        var handler = new DeleteListingCommandHandler(_listingRepoMock.Object, _uowMock.Object);

        // Тут ID користувача співпадають (1 == 1)
        var command = new DeleteListingCommand { ListingId = 10, UserId = 1 };
        var listing = new Listing { Id = 10, UserId = 1 };

        _listingRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(listing);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        // Перевіряємо, що метод DeleteAsync був викликаний саме для ID 10
        _listingRepoMock.Verify(r => r.DeleteAsync(10, It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}