namespace JobApplicationTrackerApi.Models;

public class ApplicationUser
{
    public int Id { get; set; }

    public string Identifier { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.User;

    public bool RequiresPasswordChange { get; set; }

    // Navigation property
    public List<JobApplication> JobApplications { get; set; } = new();
}
