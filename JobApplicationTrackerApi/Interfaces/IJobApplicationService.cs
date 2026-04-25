using JobApplicationTrackerApi.DTOs;

namespace JobApplicationTrackerApi.Interfaces;

public interface IJobApplicationService
{
    Task ArchiveStaleApplicationsAsync(int userId, DateOnly currentDate);

    Task<JobApplicationResponseDto> CreateAsync(int userId, CreateJobApplicationDto dto);

    Task<IEnumerable<JobApplicationResponseDto>> GetAllAsync(int userId, JobApplicationQueryDto queryDto);

    Task<JobApplicationStatsDto> GetStatsAsync(int userId);

    Task<JobApplicationResponseDto?> GetByIdAsync(int userId, int id);

    Task<JobApplicationResponseDto?> UpdateAsync(int userId, int id, UpdateJobApplicationDto dto);

    Task<bool> DeleteAsync(int userId, int id);
}
