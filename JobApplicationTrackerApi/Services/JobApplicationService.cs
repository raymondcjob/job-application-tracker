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

    public async Task<JobApplicationResponseDto> CreateAsync(int userId, CreateJobApplicationDto dto)
    {
        var jobApplication = new JobApplication
        {
            CompanyName = dto.CompanyName,
            Position = dto.Position,
            Status = dto.Status,
            DateApplied = dto.DateApplied,
            Notes = dto.Notes,
            UserId = userId
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

    

    public async Task<IEnumerable<JobApplicationResponseDto>> GetAllAsync(int userId, JobApplicationQueryDto queryDto)
    {
        var pageNumber = queryDto.PageNumber < 1 ? 1 : queryDto.PageNumber;
        var pageSize = queryDto.PageSize < 1 ? 10 : queryDto.PageSize;

        IQueryable<JobApplication> jobApplicationsQuery = _context.JobApplications
            .Where(jobApplications => jobApplications.UserId == userId);

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

    public async Task<JobApplicationResponseDto?> GetByIdAsync(int userId, int id)
    {
        var jobApplication = await _context.JobApplications
            .Where(jobApplication => jobApplication.UserId == userId && jobApplication.Id == id)
            .Select(jobApplication => new JobApplicationResponseDto
            {
                Id = jobApplication.Id,
                CompanyName = jobApplication.CompanyName,
                Position = jobApplication.Position,
                Status = jobApplication.Status,
                DateApplied = jobApplication.DateApplied,
                Notes = jobApplication.Notes
            })
            .FirstOrDefaultAsync();

        return jobApplication;
    }

    public async Task<JobApplicationResponseDto?> UpdateAsync(int userId, int id, UpdateJobApplicationDto dto)
    {
        var jobApplication = await _context.JobApplications
            .FirstOrDefaultAsync(jobApplication => jobApplication.UserId == userId && jobApplication.Id == id);

        if (jobApplication == null)
        {
            return null;
        }

        jobApplication.CompanyName = dto.CompanyName;
        jobApplication.Position = dto.Position;
        jobApplication.Status = dto.Status;
        jobApplication.DateApplied = dto.DateApplied;
        jobApplication.Notes = dto.Notes;

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

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var jobApplication = await _context.JobApplications
            .FirstOrDefaultAsync(jobApplication => jobApplication.UserId == userId && jobApplication.Id == id);

        if (jobApplication == null)
        {
            return false;
        }

        _context.JobApplications.Remove(jobApplication);
        await _context.SaveChangesAsync();

        return true;
    }
}