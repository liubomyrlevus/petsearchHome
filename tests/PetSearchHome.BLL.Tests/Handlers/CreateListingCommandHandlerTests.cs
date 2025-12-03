using FluentAssertions;
using Moq;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Handlers;
using Xunit;

namespace PetSearchHome.BLL.Tests.Handlers;

public class CreateListingCommandHandlerTests
{
    private readonly Mock<IListingRepository> _listingRepository = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task Handle_Should_MapCommand_AndPersist()
    {
        var handler = new CreateListingCommandHandler(_listingRepository.Object, _uow.Object);
        Listing? storedListing = null;

        _listingRepository.Setup(r => r.AddAsync(It.IsAny<Listing>(), It.IsAny<CancellationToken>()))
            .Callback<Listing, CancellationToken>((l, _) => storedListing = l)
            .ReturnsAsync(42);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var cmd = new CreateListingCommand
        {
            UserId = 5,
            AnimalType = AnimalType.cat,
            Breed = "British",
            AgeMonths = 12,
            Sex = AnimalSex.female,
            Size = AnimalSize.medium,
            Color = "Grey",
            City = "Lviv",
            District = "Center",
            Description = "Friendly cat",
            SpecialNeeds = "Diet food",
            HealthInfo = new HealthInfoDto
            {
                Vaccinations = "All",
                Sterilized = true,
                ChronicDiseases = "None",
                TreatmentHistory = "N/A"
            },
            PhotoUrls = new List<string> { "url1", "url2" }
        };

        var result = await handler.Handle(cmd, CancellationToken.None);

        result.Should().Be(42);
        storedListing.Should().NotBeNull();
        storedListing!.UserId.Should().Be(cmd.UserId);
        storedListing.AnimalType.Should().Be(cmd.AnimalType);
        storedListing.Photos.Should().HaveCount(2);
        storedListing.Photos.First().IsPrimary.Should().BeTrue();
        storedListing.HealthInfo.Should().NotBeNull();
        storedListing.HealthInfo!.Sterilized.Should().BeTrue();

        _listingRepository.Verify(r => r.AddAsync(It.IsAny<Listing>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
