using JobApplicationTrackerApi.DTOs;

namespace JobApplicationTrackerApi.Interfaces;

public interface IJobApplicationService
{
    Task<JobApplicationResponseDto> CreateAsync(CreateJobApplicationDto dto);

    Task<IEnumerable<JobApplicationResponseDto>> GetAllAsync(JobApplicationQueryDto queryDto);

    Task<JobApplicationResponseDto?> GetByIdAsync(int id);

    Task<JobApplicationResponseDto?> UpdateAsync(int id, UpdateJobApplicationDto dto);

    Task<bool> DeleteAsync(int id);
}