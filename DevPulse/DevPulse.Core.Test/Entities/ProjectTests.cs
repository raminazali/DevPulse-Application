using DevPulse.Core.Entities.Projects;

namespace DevPulse.Core.Test.Entities;

public class ProjectTests
{
    [Fact]
    public void Constructor_WithValidArgs_SetsProperties()
    {
        var userId = Guid.NewGuid();
        var name = "My Project";

        var project = new Project(name, userId);

        Assert.Equal(name, project.Name);
        Assert.Equal(userId, project.UserId);
        Assert.True(project.IsActive);
        Assert.True(project.CreatedAt <= DateTime.UtcNow);
        Assert.StartsWith("proj_", project.ApiKey);
        Assert.NotEmpty(project.ApiKey);
        Assert.NotNull(project.ErrorLogs);
        Assert.Empty(project.ErrorLogs);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyName_Throws(string name)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Project(name, Guid.NewGuid()));
        Assert.Contains("name", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_WithEmptyUserId_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Project("Valid", Guid.Empty));
        Assert.Contains("userId", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_TrimsName()
    {
        var project = new Project("  My Project  ", Guid.NewGuid());
        Assert.Equal("My Project", project.Name);
    }

    [Fact]
    public void UpdateName_ValidName_Updates()
    {
        var project = new Project("Original", Guid.NewGuid());
        project.UpdateName("Updated Name");
        Assert.Equal("Updated Name", project.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateName_EmptyName_Throws(string name)
    {
        var project = new Project("Original", Guid.NewGuid());
        var ex = Assert.Throws<ArgumentException>(() => project.UpdateName(name));
        Assert.Contains("name", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ChangeStatus_TogglesCorrectly()
    {
        var project = new Project("Test", Guid.NewGuid());
        Assert.True(project.IsActive);

        project.ChangeStatus(false);
        Assert.False(project.IsActive);

        project.ChangeStatus(true);
        Assert.True(project.IsActive);
    }

    [Fact]
    public void ChangeStatus_SameStatus_IsIdempotent()
    {
        var project = new Project("Test", Guid.NewGuid());
        Assert.True(project.IsActive);

        project.ChangeStatus(true);
        Assert.True(project.IsActive);
    }

    [Fact]
    public void RegenerateApiKey_GeneratesNewKey()
    {
        var project = new Project("Test", Guid.NewGuid());
        var originalKey = project.ApiKey;

        project.RegenerateApiKey();

        Assert.NotEqual(originalKey, project.ApiKey);
        Assert.StartsWith("proj_", project.ApiKey);
    }

    [Fact]
    public void Update_ChangesNameAndStatus()
    {
        var project = new Project("Original", Guid.NewGuid());
        Assert.True(project.IsActive);

        project.Update("New Name", false);

        Assert.Equal("New Name", project.Name);
        Assert.False(project.IsActive);
    }

    [Fact]
    public void Update_OnlyName_WhenStatusNotSpecified()
    {
        var project = new Project("Original", Guid.NewGuid());
        project.ChangeStatus(false);
        Assert.False(project.IsActive);

        project.Update("New Name");

        Assert.Equal("New Name", project.Name);
        Assert.False(project.IsActive);
    }

    [Fact]
    public void ApiKey_HasProjectPrefix()
    {
        var project = new Project("Test", Guid.NewGuid());
        Assert.StartsWith("proj_", project.ApiKey);
    }
}
