namespace JobApplicationTrackerApi.DTOs;

public class AdminCreateUserDto
{
    public string Identifier { get; set; } = string.Empty;
}

public class AdminUpdateUserDto
{
    public string Identifier { get; set; } = string.Empty;
}

public class AdminUserActionResponseDto
{
    public string Message { get; set; } = string.Empty;

    public string? TemporaryPassword { get; set; }
}
