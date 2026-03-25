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
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobApplicationResponseDto>>> GetAll([FromQuery] JobApplicationQueryDto queryDto)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobApplicationResponseDto>> GetById(int id)
    {
        throw new NotImplementedException();
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