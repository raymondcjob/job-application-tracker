using JobApplicationTrackerApi.Data;
using JobApplicationTrackerApi.DTOs;
using JobApplicationTrackerApi.Models;
using JobApplicationTrackerApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserSummaryDto>>> GetAll()
    {
        var users = await _context.Users
            .OrderBy(user => user.Identifier)
            .Select(user => new UserSummaryDto
            {
                Id = user.Id,
                Identifier = user.Identifier,
                Role = user.Role.ToString(),
                RequiresPasswordChange = user.RequiresPasswordChange
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<AdminUserActionResponseDto>> Create(AdminCreateUserDto dto)
    {
        var identifier = dto.Identifier.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(identifier))
        {
            return BadRequest(new { message = "email / username is required" });
        }

        var exists = await _context.Users.AnyAsync(user => user.Identifier == identifier);

        if (exists)
        {
            return BadRequest(new { message = "email / username is already registered" });
        }

        var temporaryPassword = GenerateTemporaryPassword();

        var user = new ApplicationUser
        {
            Identifier = identifier,
            PasswordHash = PasswordHasher.Hash(temporaryPassword),
            Role = UserRole.User,
            RequiresPasswordChange = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new AdminUserActionResponseDto
        {
            Message = $"user {identifier} created",
            TemporaryPassword = temporaryPassword
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AdminUserActionResponseDto>> Update(int id, AdminUpdateUserDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(existingUser => existingUser.Id == id);

        if (user == null)
        {
            return NotFound(new { message = "user not found" });
        }

        var identifier = dto.Identifier.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(identifier))
        {
            return BadRequest(new { message = "email / username is required" });
        }

        var duplicate = await _context.Users.AnyAsync(existingUser => existingUser.Id != id && existingUser.Identifier == identifier);

        if (duplicate)
        {
            return BadRequest(new { message = "email / username is already registered" });
        }

        user.Identifier = identifier;
        await _context.SaveChangesAsync();

        return Ok(new AdminUserActionResponseDto
        {
            Message = $"user {identifier} updated"
        });
    }

    [HttpPost("{id}/reset-password")]
    public async Task<ActionResult<AdminUserActionResponseDto>> ResetPassword(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(existingUser => existingUser.Id == id);

        if (user == null)
        {
            return NotFound(new { message = "user not found" });
        }

        var temporaryPassword = GenerateTemporaryPassword();
        user.PasswordHash = PasswordHasher.Hash(temporaryPassword);
        user.RequiresPasswordChange = true;

        await _context.SaveChangesAsync();

        return Ok(new AdminUserActionResponseDto
        {
            Message = $"temporary password reset for {user.Identifier}",
            TemporaryPassword = temporaryPassword
        });
    }

    private static string GenerateTemporaryPassword()
    {
        return Random.Shared.Next(100000, 1000000).ToString();
    }
}
