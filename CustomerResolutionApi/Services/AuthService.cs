using AutoMapper;
using CustomerResolutionApi.Authentication;
using CustomerResolutionApi.DTOs;
using CustomerResolutionApi.Models;
using CustomerResolutionApi.Models.Enums;
using CustomerResolutionApi.Repositories.Interfaces;
using CustomerResolutionApi.Services.Interfaces;

namespace CustomerResolutionApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtHelper _jwtHelper;
    private readonly IMapper _mapper;

    public AuthService(IUserRepository userRepository, IJwtHelper jwtHelper, IMapper mapper)
    {
        _userRepository = userRepository;
        _jwtHelper = jwtHelper;
        _mapper = mapper;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("A user with this email already exists.");

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.Customer
        };

        await _userRepository.CreateAsync(user);

        var token = _jwtHelper.GenerateToken(user);
        return new AuthResponse
        {
            Token = token,
            User = _mapper.Map<UserResponse>(user)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = _jwtHelper.GenerateToken(user);
        return new AuthResponse
        {
            Token = token,
            User = _mapper.Map<UserResponse>(user)
        };
    }

    public async Task<UserResponse> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
        return _mapper.Map<UserResponse>(user);
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserResponse>>(users);
    }

    public async Task<UserResponse> UpdateUserRoleAsync(int userId, UpdateUserRoleRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        if (!Enum.TryParse<Role>(request.Role, out var role))
            throw new ArgumentException("Invalid role. Must be Admin, SupportEngineer, or Customer.");

        user.Role = role;
        await _userRepository.UpdateAsync(user);
        return _mapper.Map<UserResponse>(user);
    }
}
