namespace JobApplicationTrackerApi.Models;

public class JobApplication
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string Position { get; set; } = string.Empty;

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;

    public DateTime DateApplied { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public string? Notes { get; set; }

    // Foreign key
    public int UserId { get; set; }

    // Navigation property
    public ApplicationUser User { get; set; } = null!;
}