using Moq;
using Xunit;
using PetSearchHome.BLL.Handlers;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace PetSearchHome.Tests;

public class ReportTests
{
    private readonly Mock<IReportRepository> _reportRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly CreateReportCommandHandler _handler;

    public ReportTests()
    {
        _reportRepoMock = new Mock<IReportRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _handler = new CreateReportCommandHandler(_reportRepoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Report()
    {
        var command = new CreateReportCommand
        {
            ReporterId = 1,
            ReportedType = ReportTargetType.listing,
            ReportedEntityId = 5,
            Reason = "Spam"
        };

        await _handler.Handle(command, CancellationToken.None);

        _reportRepoMock.Verify(r => r.AddAsync(It.Is<Report>(x =>
            x.Reason == "Spam" && x.Status == ReportStatus.pending),
            It.IsAny<CancellationToken>()), Times.Once);

        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}