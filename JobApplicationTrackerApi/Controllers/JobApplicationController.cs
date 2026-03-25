using Microsoft.AspNetCore.Mvc;
using JobApplicationTrackerApi.DTOs;
using JobApplicationTrackerApi.Interfaces;

namespace JobApplicationTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobApplicationsController : ControllerBase
{
    private readonly IJobApplicationService _jobApplicationService;

    public JobApplicationsController(IJobApplicationService jobApplicationService)
    {
        _jobApplicationService = jobApplicationService;
    }

    [HttpPost]
    public async Task<ActionResult<JobApplicationResponseDto>> Create(CreateJobApplicationDto dto)
    {
        var createdJobApplication = await _jobApplicationService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetById), new { id = createdJobApplication.Id }, createdJobApplication);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobApplicationResponseDto>>> GetAll([FromQuery] JobApplicationQueryDto queryDto)
    {
        var jobApplications = await _jobApplicationService.GetAllAsync(queryDto);

        return Ok(jobApplications);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobApplicationResponseDto>> GetById(int id)
    {
        var jobApplication = await _jobApplicationService.GetByIdAsync(id);

        if (jobApplication == null)
        {
            return NotFound(new { message = $"job application with id {id} not found" });
        }

        return Ok(jobApplication);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<JobApplicationResponseDto>> Update(int id, UpdateJobApplicationDto dto)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        throw new NotImplementedException();
    }
}