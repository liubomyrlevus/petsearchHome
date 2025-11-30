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

public class ReviewTests
{
    private readonly Mock<IReviewRepository> _reviewRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;

    public ReviewTests()
    {
        _reviewRepoMock = new Mock<IReviewRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _uowMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task CreateReview_Should_Throw_If_Self_Review()
    {
        var handler = new CreateReviewCommandHandler(_reviewRepoMock.Object, _userRepoMock.Object, _uowMock.Object);
        var command = new CreateReviewCommand { ReviewerId = 1, ReviewedId = 1, Rating = 5 }; // Сам собі

        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateReview_Should_Save_Valid_Review()
    {
        var handler = new CreateReviewCommandHandler(_reviewRepoMock.Object, _userRepoMock.Object, _uowMock.Object);
        var command = new CreateReviewCommand { ReviewerId = 1, ReviewedId = 2, Rating = 5, Comment = "Good!" };

        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RegisteredUser { Id = 1, IndividualProfile = new IndividualProfile { FirstName = "Max" } });

        var result = await handler.Handle(command, CancellationToken.None);

        _reviewRepoMock.Verify(r => r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal("Good!", result.Comment);
    }
}