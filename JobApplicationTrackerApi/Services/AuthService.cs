using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JobApplicationTrackerApi.Data;
using JobApplicationTrackerApi.DTOs;
using JobApplicationTrackerApi.Interfaces;
using JobApplicationTrackerApi.Models;
using JobApplicationTrackerApi.Utilities;

namespace JobApplicationTrackerApi.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        var identifier = dto.EmailOrUsername.Trim().ToLower();

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(user => user.Identifier.ToLower() == identifier);

        if (existingUser != null)
        {
            return null;
        }

        var user = new ApplicationUser
        {
            Identifier = identifier,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            Role = UserRole.User
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Identifier = user.Identifier,
            Role = user.Role.ToString(),
            RequiresPasswordChange = user.RequiresPasswordChange
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var identifier = dto.EmailOrUsername.Trim().ToLower();
        var passwordHash = PasswordHasher.Hash(dto.Password);

        var user = await _context.Users
            .FirstOrDefaultAsync(user => user.Identifier.ToLower() == identifier && user.PasswordHash == passwordHash);

        if (user == null)
        {
            return null;
        }

        if (user.Role == UserRole.User)
        {
            await ArchiveStaleApplicationsAsync(user.Id);
        }

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Identifier = user.Identifier,
            Role = user.Role.ToString(),
            RequiresPasswordChange = user.RequiresPasswordChange
        };
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(existingUser => existingUser.Id == userId);

        if (user == null)
        {
            throw new InvalidOperationException("user not found");
        }

        user.PasswordHash = PasswordHasher.Hash(dto.NewPassword);
        user.RequiresPasswordChange = false;

        await _context.SaveChangesAsync();
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("jwt key is not configurated");
        
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Identifier),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task ArchiveStaleApplicationsAsync(int userId)
    {
        var archiveCutoff = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-14));

        var staleApplications = await _context.JobApplications
            .Where(jobApplication =>
                jobApplication.UserId == userId &&
                jobApplication.Status == ApplicationStatus.Applied &&
                jobApplication.DateApplied <= archiveCutoff)
            .ToListAsync();

        if (staleApplications.Count == 0)
        {
            return;
        }

        foreach (var jobApplication in staleApplications)
        {
            jobApplication.Status = ApplicationStatus.Archived;
        }

        await _context.SaveChangesAsync();
    }
}
