using JobApplicationTrackerApi.DTOs;

namespace JobApplicationTrackerApi.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);

    Task<AuthResponseDto?> LoginAsync(LoginDto dto);

    Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
}
