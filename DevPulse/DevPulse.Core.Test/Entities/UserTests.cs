using DevPulse.Core.Entities.Projects;

namespace DevPulse.Core.Test.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_WithValidArgs_SetsProperties()
    {
        var user = new User("test@example.com", "John Doe", false);

        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("John Doe", user.FullName);
        Assert.False(user.IsAdmin);
        Assert.True(user.IsActive);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
        Assert.Null(user.LastLoginAt);
        Assert.Null(user.PasswordHash);
        Assert.NotNull(user.Projects);
        Assert.Empty(user.Projects);
    }

    [Fact]
    public void Constructor_DefaultIsAdmin_IsFalse()
    {
        var user = new User("test@example.com", "John Doe");
        Assert.False(user.IsAdmin);
    }

    [Fact]
    public void Constructor_WithIsAdminTrue_SetsAdmin()
    {
        var user = new User("admin@example.com", "Admin", true);
        Assert.True(user.IsAdmin);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyEmail_Throws(string email)
    {
        var ex = Assert.Throws<ArgumentException>(() => new User(email, "Name"));
        Assert.Contains("email", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("user@")]
    [InlineData("@domain.com")]
    [InlineData("user@.com")]
    public void Constructor_InvalidEmailFormat_Throws(string email)
    {
        Assert.Throws<ArgumentException>(() => new User(email, "Name"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyFullName_Throws(string fullName)
    {
        var ex = Assert.Throws<ArgumentException>(() => new User("test@test.com", fullName));
        Assert.Contains("fullname", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_LowercasesEmail()
    {
        var user = new User("Test@Example.COM", "Name");
        Assert.Equal("test@example.com", user.Email);
    }

    [Fact]
    public void Constructor_TrimsFullName()
    {
        var user = new User("test@test.com", "  John Doe  ");
        Assert.Equal("John Doe", user.FullName);
    }

    [Fact]
    public void ChangeEmail_ValidEmail_Updates()
    {
        var user = new User("old@test.com", "Name");
        user.ChangeEmail("new@test.com");
        Assert.Equal("new@test.com", user.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangeEmail_EmptyEmail_Throws(string email)
    {
        var user = new User("old@test.com", "Name");
        var ex = Assert.Throws<ArgumentException>(() => user.ChangeEmail(email));
        Assert.Contains("email", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ChangeEmail_InvalidFormat_Throws()
    {
        var user = new User("old@test.com", "Name");
        Assert.Throws<ArgumentException>(() => user.ChangeEmail("invalid"));
    }

    [Fact]
    public void ChangePassword_ValidHash_Updates()
    {
        var user = new User("test@test.com", "Name");
        user.ChangePassword("hashed_value");
        Assert.Equal("hashed_value", user.PasswordHash);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangePassword_EmptyHash_Throws(string hash)
    {
        var user = new User("test@test.com", "Name");
        var ex = Assert.Throws<ArgumentException>(() => user.ChangePassword(hash));
        Assert.Contains("passwordhash", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ChangeStatus_TogglesCorrectly()
    {
        var user = new User("test@test.com", "Name");
        Assert.True(user.IsActive);

        user.ChangeStatus(false);
        Assert.False(user.IsActive);

        user.ChangeStatus(true);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void ChangeStatus_SameStatus_IsIdempotent()
    {
        var user = new User("test@test.com", "Name");
        Assert.True(user.IsActive);

        user.ChangeStatus(true);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void RecordLogin_SetsLastLoginAt()
    {
        var user = new User("test@test.com", "Name");
        Assert.Null(user.LastLoginAt);

        user.RecordLogin();

        Assert.NotNull(user.LastLoginAt);
        Assert.True(user.LastLoginAt <= DateTime.UtcNow);
    }

    [Fact]
    public void MakeAdmin_SetsIsAdminTrue()
    {
        var user = new User("test@test.com", "Name", false);
        Assert.False(user.IsAdmin);

        user.MakeAdmin();
        Assert.True(user.IsAdmin);
    }

    [Fact]
    public void RemoveAdmin_SetsIsAdminFalse()
    {
        var user = new User("test@test.com", "Name", true);
        Assert.True(user.IsAdmin);

        user.RemoveAdmin();
        Assert.False(user.IsAdmin);
    }

    [Fact]
    public void UpdateFullName_ValidName_Updates()
    {
        var user = new User("test@test.com", "Old Name");
        user.UpdateFullName("New Name");
        Assert.Equal("New Name", user.FullName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateFullName_EmptyName_Throws(string name)
    {
        var user = new User("test@test.com", "Name");
        var ex = Assert.Throws<ArgumentException>(() => user.UpdateFullName(name));
        Assert.Contains("fullname", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Update_ChangesFullNameAndEmailAndStatus()
    {
        var user = new User("old@test.com", "Old Name");
        user.Update("New Name", "new@test.com", false);

        Assert.Equal("New Name", user.FullName);
        Assert.Equal("new@test.com", user.Email);
        Assert.False(user.IsActive);
    }

    [Fact]
    public void Update_OnlyFullName_WhenOptionalParamsOmitted()
    {
        var user = new User("old@test.com", "Old Name");
        user.Update("New Name");

        Assert.Equal("New Name", user.FullName);
        Assert.Equal("old@test.com", user.Email);
        Assert.True(user.IsActive);
    }
}
