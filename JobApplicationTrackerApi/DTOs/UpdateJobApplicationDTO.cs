using JobApplicationTrackerApi.Models;

namespace JobApplicationTrackerApi.DTOs;

public class UpdateJobApplicationDto
{
    public string CompanyName { get; set; } = string.Empty;

    public string Position { get; set; } = string.Empty;

    public ApplicationStatus Status { get; set; }

    public DateOnly DateApplied { get; set; }

    public string? Notes { get; set; }
}