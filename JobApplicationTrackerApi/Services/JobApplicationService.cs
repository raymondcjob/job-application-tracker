using JobApplicationTrackerApi.DTOs;
using JobApplicationTrackerApi.Interfaces;

namespace JobApplicationTrackerApi.Services;

public class JobApplicationService : IJobApplicationService
{
    public Task<JobApplicationResponseDto> CreateAsync(CreateJobApplicationDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<JobApplicationResponseDto>> GetAllAsync(JobApplicationQueryDto queryDto)
    {
        throw new NotImplementedException();
    }

    public Task<JobApplicationResponseDto?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<JobApplicationResponseDto?> UpdateAsync(int id, UpdateJobApplicationDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}