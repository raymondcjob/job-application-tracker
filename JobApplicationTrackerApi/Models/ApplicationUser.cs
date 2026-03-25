namespace JobApplicationTrackerApi.Models;

public class ApplicationUser
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    // Navigation property
    public List<JobApplication> JobApplications { get; set; } = new();
}