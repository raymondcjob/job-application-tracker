using JobApplicationTrackerApi.DTOs;
using JobApplicationTrackerApi.Interfaces;

namespace JobApplicationTrackerApi.Services;

public class AuthService : IAuthService
{
    public Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        throw new NotImplementedException();
    }
}