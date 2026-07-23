using DevPulse.Application.Common;
using DevPulse.Application.DTOs.Projects;
using DevPulse.Application.Services.Projects.Inetfaces;
using DevPulse.Application.Services.Projects;
using DevPulse.Application.Specifications.Users;
using DevPulse.Core.Entities.Projects;
using DevPulse.Core.Exceptions;
using DevPulse.Infrastructure.Repository.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DevPulse.Application.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IRepository<User>> _userRepositoryMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IRepository<User>>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();

            // Set up a real configuration with JWT settings
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"JwtSettings:Key", "this_is_a_secure_key_with_at_least_32_characters_long"},
                {"JwtSettings:Issuer", "test_issuer"},
                {"JwtSettings:Audience", "test_audience"}
            });
            _configuration = configurationBuilder.Build();

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _configuration);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var email = "test@example.com";
            var password = "Password123!";
            var userId = Guid.NewGuid();
            var fullName = "Test User";

            var user = new User(email, fullName, false)
            {
                Id = userId
            };
            user.ChangePassword("hashed_password");

            var loginRequest = new LoginRequestDTO
            {
                Email = email,
                Password = password
            };

            // Setup mocks
            _userRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByEmailSpec>()))
                              .ReturnsAsync(user);

            _passwordHasherMock.Setup(p => p.VerifyHashedPassword(
                    It.IsAny<User>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                              .Returns(PasswordVerificationResult.Success);

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.Equal(DateTime.UtcNow.AddDays(1).Date, result.ExpiresIn.Date); // Token expires in 1 day
            Assert.Equal(userId, result.User.Id);
            Assert.Equal(email, result.User.Email);
            Assert.Equal(fullName, result.User.FullName);
            Assert.False(result.User.IsAdmin);
            Assert.True(result.User.IsActive);
            Assert.Equal(user.CreatedAt.Date, result.User.CreatedAt.Date);

            // Verify that RecordLogin and UpdateAsync were called
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenUserNotFound()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var password = "Password123!";

            var loginRequest = new LoginRequestDTO
            {
                Email = email,
                Password = password
            };

            // Setup mocks
            _userRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByEmailSpec>()))
                              .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _authService.LoginAsync(loginRequest));

            Assert.Equal(ErrorMessages.UserNotFound, exception.Message);
            Assert.Equal("AUTH_USER_NOT_FOUND", exception.Code);

            // Verify that password hasher was not called
            _passwordHasherMock.Verify(p => p.VerifyHashedPassword(
                    It.IsAny<User>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenPasswordIsInvalid()
        {
            // Arrange
            var email = "test@example.com";
            var password = "WrongPassword!";
            var userId = Guid.NewGuid();
            var fullName = "Test User";

            var user = new User(email, fullName, false)
            {
                Id = userId
            };
            user.ChangePassword("hashed_password");

            var loginRequest = new LoginRequestDTO
            {
                Email = email,
                Password = password
            };

            // Setup mocks
            _userRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByEmailSpec>()))
                              .ReturnsAsync(user);

            _passwordHasherMock.Setup(p => p.VerifyHashedPassword(
                    It.IsAny<User>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                              .Returns(PasswordVerificationResult.Failed);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _authService.LoginAsync(loginRequest));

            Assert.Equal(ErrorMessages.InvalidPassword, exception.Message);
            Assert.Equal("AUTH_INVALID_PASSWORD", exception.Code);

            // Verify that UpdateAsync was not called (since password verification failed)
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }
    }
}