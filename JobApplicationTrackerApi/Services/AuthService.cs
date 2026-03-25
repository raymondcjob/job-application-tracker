using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using JobApplicationTrackerApi.Data;
using JobApplicationTrackerApi.DTOs;
using JobApplicationTrackerApi.Interfaces;
using JobApplicationTrackerApi.Models;

namespace JobApplicationTrackerApi.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(user => user.Email.ToLower() == email);

        if (existingUser != null)
        {
            return null;
        }

        var user = new ApplicationUser
        {
            Email = email,
            PasswordHash = HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = "temporary-placeholder",
            Email = user.Email
        };
    }

    public Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        throw new NotImplementedException();
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = sha256.ComputeHash(bytes);

        return Convert.ToBase64String(hashBytes);
    }

}