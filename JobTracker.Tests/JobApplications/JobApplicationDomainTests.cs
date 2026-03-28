using FluentAssertions;
using JobTracker.Domain.Entities;
using JobTracker.Domain.Enums;
using JobTracker.Domain.Exceptions;

namespace JobTracker.Tests.JobApplications;

public class JobApplicationDomainTests
{
    [Fact]
    public void Create_WithValidData_SetsDefaultStatusToApplied()
    {
        var app = JobApplication.Create("user-1", "Acme Corp", "Engineer", DateTime.UtcNow);

        app.Status.Should().Be(ApplicationStatus.Applied);
    }

    [Fact]
    public void Create_WithEmptyCompanyName_ThrowsDomainException()
    {
        var act = () => JobApplication.Create("user-1", "", "Engineer", DateTime.UtcNow);

        act.Should().Throw<DomainException>().WithMessage("*Company name*");
    }

    [Fact]
    public void Create_WithEmptyJobTitle_ThrowsDomainException()
    {
        var act = () => JobApplication.Create("user-1", "Acme Corp", "", DateTime.UtcNow);

        act.Should().Throw<DomainException>().WithMessage("*Job title*");
    }

    [Fact]
    public void UpdateStatus_ChangesStatusAndUpdatedAt()
    {
        var app = JobApplication.Create("user-1", "Acme Corp", "Engineer", DateTime.UtcNow);
        var before = app.UpdatedAt;

        app.UpdateStatus(ApplicationStatus.Interview);

        app.Status.Should().Be(ApplicationStatus.Interview);
        app.UpdatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void Update_WithEmptyCompanyName_ThrowsDomainException()
    {
        var app = JobApplication.Create("user-1", "Acme Corp", "Engineer", DateTime.UtcNow);

        var act = () => app.Update("", "Engineer", null, DateTime.UtcNow, null, null);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AttachCv_SetsFileProperties()
    {
        var app = JobApplication.Create("user-1", "Acme Corp", "Engineer", DateTime.UtcNow);

        app.AttachCv("cv.pdf", "/uploads/cv.pdf");

        app.CvFileName.Should().Be("cv.pdf");
        app.CvFilePath.Should().Be("/uploads/cv.pdf");
    }

    [Fact]
    public void MarkReminderSent_SetsReminderSentTrue()
    {
        var app = JobApplication.Create("user-1", "Acme Corp", "Engineer", DateTime.UtcNow,
            reminderDate: DateTime.UtcNow.AddDays(1));

        app.MarkReminderSent();

        app.ReminderSent.Should().BeTrue();
    }

    [Fact]
    public void Create_AssignsNewGuid()
    {
        var app1 = JobApplication.Create("user-1", "Acme", "Eng", DateTime.UtcNow);
        var app2 = JobApplication.Create("user-1", "Beta", "Dev", DateTime.UtcNow);

        app1.Id.Should().NotBe(app2.Id);
    }
}
