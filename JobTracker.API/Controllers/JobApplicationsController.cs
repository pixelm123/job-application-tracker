using JobTracker.API.Extensions;
using JobTracker.Application.Interviews.Commands.CreateInterview;
using JobTracker.Application.JobApplications.Commands.CreateJobApplication;
using JobTracker.Application.JobApplications.Commands.DeleteJobApplication;
using JobTracker.Application.JobApplications.Commands.UpdateJobApplication;
using JobTracker.Application.JobApplications.Commands.UpdateStatus;
using JobTracker.Application.JobApplications.Commands.UploadCv;
using JobTracker.Application.JobApplications.Queries.GetJobApplicationById;
using JobTracker.Application.JobApplications.Queries.GetJobApplications;
using JobTracker.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.API.Controllers;

[ApiController]
[Route("api/applications")]
[Authorize]
public class JobApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? search = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken ct = default)
    {
        var userId = User.GetUserId();

        ApplicationStatus? statusEnum = status is not null && Enum.TryParse<ApplicationStatus>(status, out var s)
            ? s : null;

        var result = await _mediator.Send(
            new GetJobApplicationsQuery(userId, page, pageSize, statusEnum, search, fromDate, toDate), ct);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetJobApplicationByIdQuery(id, User.GetUserId()), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApplicationRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateJobApplicationCommand(
            User.GetUserId(),
            request.CompanyName,
            request.JobTitle,
            request.JobUrl,
            request.AppliedDate,
            request.Notes,
            request.ReminderDate), ct);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateApplicationRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateJobApplicationCommand(
            id,
            User.GetUserId(),
            request.CompanyName,
            request.JobTitle,
            request.JobUrl,
            request.AppliedDate,
            request.Notes,
            request.ReminderDate), ct);

        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id, [FromBody] UpdateStatusRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<ApplicationStatus>(request.Status, out var status))
            return BadRequest($"Invalid status: {request.Status}");

        await _mediator.Send(new UpdateStatusCommand(id, User.GetUserId(), status), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteJobApplicationCommand(id, User.GetUserId()), ct);
        return NoContent();
    }

    [HttpGet("{id:guid}/cv")]
    public async Task<IActionResult> DownloadCv(Guid id, CancellationToken ct)
    {
        var app = await _mediator.Send(new GetJobApplicationByIdQuery(id, User.GetUserId()), ct);

        if (app is null) return NotFound();
        if (string.IsNullOrEmpty(app.CvFilePath) || !System.IO.File.Exists(app.CvFilePath))
            return NotFound("No CV attached to this application.");

        var bytes = await System.IO.File.ReadAllBytesAsync(app.CvFilePath, ct);
        return File(bytes, "application/pdf", app.CvFileName ?? "cv.pdf");
    }

    [HttpPost("{id:guid}/cv")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadCv(Guid id, IFormFile file, CancellationToken ct)
    {
        if (file.ContentType != "application/pdf")
            return BadRequest("Only PDF files are accepted.");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File must be under 5MB.");

        await _mediator.Send(
            new UploadCvCommand(id, User.GetUserId(), file.FileName, file.OpenReadStream()), ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/interviews")]
    public async Task<IActionResult> CreateInterview(
        Guid id, [FromBody] CreateInterviewRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<InterviewType>(request.Type, out var type))
            return BadRequest($"Invalid interview type: {request.Type}");

        var result = await _mediator.Send(new CreateInterviewCommand(
            id,
            User.GetUserId(),
            request.ScheduledAt,
            type,
            request.Notes), ct);

        return CreatedAtAction(nameof(GetById), new { id }, result);
    }
}

public record CreateApplicationRequest(
    string CompanyName,
    string JobTitle,
    string? JobUrl,
    DateTime AppliedDate,
    string? Notes,
    DateTime? ReminderDate);

public record UpdateApplicationRequest(
    string CompanyName,
    string JobTitle,
    string? JobUrl,
    DateTime AppliedDate,
    string? Notes,
    DateTime? ReminderDate);

public record UpdateStatusRequest(string Status);

public record CreateInterviewRequest(
    DateTime ScheduledAt,
    string Type,
    string? Notes);
