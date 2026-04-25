namespace JobApplicationTrackerApi.DTOs;

public class UserSummaryDto
{
    public int Id { get; set; }

    public string Identifier { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public bool RequiresPasswordChange { get; set; }
}
