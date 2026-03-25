using JobApplicationTrackerApi.Models;

namespace JobApplicationTrackerApi.DTOs;

public class CreateJobApplicationDto
{
    public string CompanyName { get; set; } = string.Empty;

    public string Position { get; set; } = string.Empty;

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;

    public DateOnly DateApplied { get; set; }

    public string? Notes { get; set; }
}