using DevPulse.Application.Common;
using DevPulse.Application.DTOs.Projects;
using DevPulse.Application.Services.Projects.Inetfaces;
using DevPulse.Application.Services.Users;
using DevPulse.Application.Specifications.Users;
using DevPulse.Core.Entities.Projects;
using DevPulse.Core.Exceptions;
using DevPulse.Infrastructure.Repository.Contracts;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace DevPulse.Application.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IRepository<User>> _userRepoMock;
        private readonly Mock<IProjectService> _projectServiceMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IRepository<User>>();
            _projectServiceMock = new Mock<IProjectService>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();

            _userService = new UserService(
                _userRepoMock.Object,
                _projectServiceMock.Object,
                _passwordHasherMock.Object);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateUserAndProject_WhenValidRequest()
        {
            // Arrange
            var request = new CreateUserRequest { Email = "test@example.com", FullName = "Test User", Password = "SecurePass123!" };
            var expectedUserId = Guid.NewGuid(); // شناسه‌ای که فرض می‌کنیم دیتابیس تولید می‌کند

            _passwordHasherMock.Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
                .Returns("hashed_password");

            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, _) =>
                {
                    typeof(User).GetProperty("Id")?.SetValue(user, expectedUserId);
                })
                .ReturnsAsync((User user, CancellationToken _) => user);

            var expectedProjectName = $"پروژه {request.FullName}!";
            _projectServiceMock.Setup(p => p.CreateAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new Project(expectedProjectName, expectedUserId));

            // Act
            var result = await _userService.CreateUserAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUserId, result.Id); // اکنون این خط با موفقیت پاس می‌شود
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.FullName, result.FullName);
            Assert.False(result.IsAdmin);

            _passwordHasherMock.Verify(p => p.HashPassword(It.Is<User>(u => u.Email == request.Email), request.Password), Times.Once);
            _userRepoMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == request.Email)), Times.Once);

            _projectServiceMock.Verify(p => p.CreateAsync(expectedUserId, expectedProjectName), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsUserDetailDto_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User("test@example.com", "Test User", false);
            typeof(User).GetProperty("Id")?.SetValue(user, userId);

            var project = new Project("Project 1", userId);
            typeof(Project).GetProperty("Id")?.SetValue(project, Guid.NewGuid());
            user.Projects.Add(project);

            _userRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>())).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Single(result.Projects);
            Assert.Equal("Project 1", result.Projects[0].Name);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>())).ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetByIdAsync(userId));
            Assert.Equal("USER_NOT_FOUND", exception.Code);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsPagedResult_WithCorrectData()
        {
            // Arrange
            var request = new PaginationRequest { Page = 1, PageSize = 2 };
            var user1 = new User("u1@a.com", "User 1", false);
            var user2 = new User("u2@a.com", "User 2", true);

            user1.Projects.Add(new Project("P1", Guid.NewGuid()));
            user1.Projects.Add(new Project("P2", Guid.NewGuid()));
            user2.Projects.Add(new Project("P3", Guid.NewGuid()));

            _userRepoMock.Setup(r => r.CountAsync(It.IsAny<UsersCountSpec>())).ReturnsAsync(2);
            _userRepoMock.Setup(r => r.ListAsync(It.IsAny<UsersSpec>())).ReturnsAsync(new List<User> { user1, user2 });

            // Act
            var result = await _userService.GetUsersAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(2, result.Items.First(x => x.Email == "u1@a.com").ProjectCount);
            Assert.Equal(1, result.Items.First(x => x.Email == "u2@a.com").ProjectCount);
        }
    }
}