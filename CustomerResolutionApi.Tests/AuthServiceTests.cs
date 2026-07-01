using AutoMapper;
using CustomerResolutionApi.Authentication;
using CustomerResolutionApi.DTOs;
using CustomerResolutionApi.Mappings;
using CustomerResolutionApi.Models;
using CustomerResolutionApi.Models.Enums;
using CustomerResolutionApi.Repositories.Interfaces;
using CustomerResolutionApi.Services;
using Moq;

namespace CustomerResolutionApi.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IJwtHelper> _jwtHelperMock;
    private readonly IMapper _mapper;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _jwtHelperMock = new Mock<IJwtHelper>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _authService = new AuthService(_userRepoMock.Object, _jwtHelperMock.Object, _mapper);
    }

    [Fact]
    public async Task RegisterAsync_WithNewEmail_ReturnsAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "Password123"
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _jwtHelperMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("test-token");

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-token", result.Token);
        Assert.Equal("John", result.User.FirstName);
        Assert.Equal("Customer", result.User.Role);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new RegisterRequest { Email = "existing@example.com", Password = "pass" };
        _userRepoMock.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(new User { Email = request.Email });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RegisterAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var request = new LoginRequest { Email = "john@example.com", Password = "Password123" };
        var user = new User
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
            Role = Role.Customer
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(user);
        _jwtHelperMock.Setup(j => j.GenerateToken(user)).Returns("login-token");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("login-token", result.Token);
        Assert.Equal("John", result.User.FirstName);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest { Email = "john@example.com", Password = "WrongPassword" };
        var user = new User
        {
            Email = "john@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword")
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(request));
    }

    [Fact]
    public async Task GetProfileAsync_WithValidId_ReturnsUserResponse()
    {
        // Arrange
        var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Role = Role.Customer };
        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _authService.GetProfileAsync(1);

        // Assert
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Customer", result.Role);
    }

    [Fact]
    public async Task GetProfileAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.GetProfileAsync(999));
    }
}
