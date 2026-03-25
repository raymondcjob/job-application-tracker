namespace JobApplicationTrackerApi.DTOs;

public class JobApplicationQueryDto
{
    public string? CompanyName { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public bool SortByDateDescending { get; set; } = true;
}