using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using JobApplicationTrackerApi.DTOs;
using JobApplicationTrackerApi.Interfaces;

namespace JobApplicationTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(new {message = "invalid user token"});
        }

        var createdJobApplication = await _jobApplicationService.CreateAsync(userId.Value, dto);

        return CreatedAtAction(nameof(GetById), new { id = createdJobApplication.Id }, createdJobApplication);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobApplicationResponseDto>>> GetAll([FromQuery] JobApplicationQueryDto queryDto)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(new {message = "invalid user token"});
        }

        var jobApplications = await _jobApplicationService.GetAllAsync(userId.Value, queryDto);

        return Ok(jobApplications);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobApplicationResponseDto>> GetById(int id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(new {message = "invalid user token"});
        }

        var jobApplication = await _jobApplicationService.GetByIdAsync(userId.Value, id);

        if (jobApplication == null)
        {
            return NotFound(new { message = $"job application with id {id} not found" });
        }

        return Ok(jobApplication);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<JobApplicationResponseDto>> Update(int id, UpdateJobApplicationDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(new {message = "invalid user token"});
        }

        var updatedJobApplication = await _jobApplicationService.UpdateAsync(userId.Value, id, dto);

        if (updatedJobApplication == null)
        {
            return NotFound(new { message = $"job application with id {id} not found" });
        }

        return Ok(updatedJobApplication);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(new {message = "invalid user token"});
        }

        var deleted = await _jobApplicationService.DeleteAsync(userId.Value, id);

        if (!deleted)
        {
            return NotFound(new { message = $"job application with id {id} not found" });
        }

        return Ok(new { message = $"job application with id {id} is now deleted" });
    }

    private int? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return null;
    }
}