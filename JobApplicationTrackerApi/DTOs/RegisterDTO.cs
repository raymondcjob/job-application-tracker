namespace JobApplicationTrackerApi.DTOs;

public class RegisterDto
{
    public string EmailOrUsername { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
