using Microsoft.AspNetCore.Mvc;
using JobApplicationTrackerApi.DTOs;
using JobApplicationTrackerApi.Interfaces;

namespace JobApplicationTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (result == null)
        {
            return BadRequest(new { message = "email is already registered" });
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        throw new NotImplementedException();
    }
}