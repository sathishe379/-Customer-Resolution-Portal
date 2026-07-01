using CustomerResolutionApi.DTOs;

namespace CustomerResolutionApi.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserResponse> GetProfileAsync(int userId);
    Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    Task<UserResponse> UpdateUserRoleAsync(int userId, UpdateUserRoleRequest request);
}
