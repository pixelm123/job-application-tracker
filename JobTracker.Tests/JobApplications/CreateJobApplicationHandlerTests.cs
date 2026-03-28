using FluentAssertions;
using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.JobApplications.Commands.CreateJobApplication;
using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace JobTracker.Tests.JobApplications;

public class CreateJobApplicationHandlerTests
{
    private static IApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TestDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesApplicationAndInvalidatesCache()
    {
        var context = CreateContext();
        var cache = new Mock<ICacheService>();
        var handler = new CreateJobApplicationCommandHandler(context, cache.Object);

        var command = new CreateJobApplicationCommand(
            "user-1", "Acme Corp", "Software Engineer", null,
            DateTime.UtcNow.AddDays(-1), "Great role", null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.CompanyName.Should().Be("Acme Corp");
        result.Status.Should().Be("Applied");
        cache.Verify(c => c.RemoveAsync("applications:user-1:p20", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_PersistsToDatabase()
    {
        var context = CreateContext();
        var cache = new Mock<ICacheService>();
        var handler = new CreateJobApplicationCommandHandler(context, cache.Object);

        await handler.Handle(
            new CreateJobApplicationCommand("user-1", "Beta Ltd", "Dev", null, DateTime.UtcNow, null, null),
            CancellationToken.None);

        var saved = await context.JobApplications.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.CompanyName.Should().Be("Beta Ltd");
    }
}
