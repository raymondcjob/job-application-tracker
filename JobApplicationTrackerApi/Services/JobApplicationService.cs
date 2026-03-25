using Microsoft.EntityFrameworkCore;
using JobApplicationTrackerApi.Data;
using JobApplicationTrackerApi.DTOs;
using JobApplicationTrackerApi.Models;
using JobApplicationTrackerApi.Interfaces;

namespace JobApplicationTrackerApi.Services;

public class JobApplicationService : IJobApplicationService
{
    private readonly ApplicationDbContext _context;

    public JobApplicationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobApplicationResponseDto> CreateAsync(CreateJobApplicationDto dto)
    {
        var jobApplication = new JobApplication
        {
            CompanyName = dto.CompanyName,
            Position = dto.Position,
            Status = dto.Status,
            DateApplied = dto.DateApplied,
            Notes = dto.Notes,
            UserId = 1
        };

        _context.JobApplications.Add(jobApplication);
        await _context.SaveChangesAsync();

        return new JobApplicationResponseDto
        {
            Id = jobApplication.Id,
            CompanyName = jobApplication.CompanyName,
            Position = jobApplication.Position,
            Status = jobApplication.Status,
            DateApplied = jobApplication.DateApplied,
            Notes = jobApplication.Notes
        };
    }

    

    public async Task<IEnumerable<JobApplicationResponseDto>> GetAllAsync(JobApplicationQueryDto queryDto)
    {
        var pageNumber = queryDto.PageNumber < 1 ? 1 : queryDto.PageNumber;
        var pageSize = queryDto.PageSize < 1 ? 10 : queryDto.PageSize;

        IQueryable<JobApplication> jobApplicationsQuery = _context.JobApplications;

        if (!string.IsNullOrWhiteSpace(queryDto.CompanyName))
        {
            var companyName = queryDto.CompanyName.Trim().ToLower();

            jobApplicationsQuery = jobApplicationsQuery
                .Where(jobApplication => jobApplication.CompanyName.ToLower().Contains(companyName));
        }

        jobApplicationsQuery = queryDto.SortByDateDescending
        ? jobApplicationsQuery.OrderByDescending(jobApplication => jobApplication.DateApplied)
        : jobApplicationsQuery.OrderBy(jobApplication => jobApplication.DateApplied);


        var jobApplications = await jobApplicationsQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(jobApplication => new JobApplicationResponseDto
            {
                Id = jobApplication.Id,
                CompanyName = jobApplication.CompanyName,
                Position = jobApplication.Position,
                Status = jobApplication.Status,
                DateApplied = jobApplication.DateApplied,
                Notes = jobApplication.Notes
            })
            .ToListAsync();

        return jobApplications;
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