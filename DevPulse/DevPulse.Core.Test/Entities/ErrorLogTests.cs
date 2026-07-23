using DevPulse.Core.Entities.Errors;

namespace DevPulse.Core.Test.Entities;

public class ErrorLogTests
{
    private readonly Guid _projectId = Guid.NewGuid();
    private readonly DateTime _now = DateTime.UtcNow;

    [Fact]
    public void Create_WithValidArgs_SetsAllProperties()
    {
        var errorLog = ErrorLog.Create(
            _projectId,
            "Test error",
            "at StackTrace()",
            "http://example.com",
            "InvalidOperationException",
            "GET",
            "{\"key\": \"value\"}",
            "?id=1",
            Guid.NewGuid(),
            "Chrome",
            "127.0.0.1",
            _now);

        Assert.NotEqual(Guid.Empty, errorLog.Id);
        Assert.Equal(_projectId, errorLog.ProjectId);
        Assert.Equal("Test error", errorLog.Message);
        Assert.Equal("at StackTrace()", errorLog.StackTrace);
        Assert.Equal("http://example.com", errorLog.Url);
        Assert.Equal("InvalidOperationException", errorLog.ExceptionType);
        Assert.Equal("GET", errorLog.Method);
        Assert.Equal("{\"key\": \"value\"}", errorLog.RequestBody);
        Assert.Equal("?id=1", errorLog.QueryString);
        Assert.NotNull(errorLog.UserId);
        Assert.Equal("Chrome", errorLog.Browser);
        Assert.Equal("127.0.0.1", errorLog.IpAddress);
        Assert.Equal(_now, errorLog.CreatedAt);
    }

    [Fact]
    public void Create_WithNullOptionalFields_SetsDefaults()
    {
        var errorLog = ErrorLog.Create(
            _projectId,
            "Test error",
            null!,
            null!,
            null!,
            null!,
            null!,
            null,
            null,
            null,
            null,
            _now);

        Assert.Equal(string.Empty, errorLog.StackTrace);
        Assert.Equal(string.Empty, errorLog.Url);
        Assert.Equal(string.Empty, errorLog.ExceptionType);
        Assert.Equal(string.Empty, errorLog.Method);
        Assert.Equal(string.Empty, errorLog.RequestBody);
        Assert.Null(errorLog.QueryString);
        Assert.Null(errorLog.UserId);
        Assert.Null(errorLog.Browser);
        Assert.Null(errorLog.IpAddress);
    }

    [Fact]
    public void Create_WithEmptyProjectId_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            ErrorLog.Create(Guid.Empty, "msg", "", "", "", "", "", null, null, null, null, _now));
        Assert.Contains("projectId", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyMessage_Throws(string? message)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            ErrorLog.Create(_projectId, message, "", "", "", "", "", null, null, null, null, _now));
        Assert.Contains("message", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var log1 = ErrorLog.Create(_projectId, "msg1", "", "", "", "", "", null, null, null, null, _now);
        var log2 = ErrorLog.Create(_projectId, "msg2", "", "", "", "", "", null, null, null, null, _now);

        Assert.NotEqual(log1.Id, log2.Id);
    }

    [Fact]
    public void Create_NullQueryString_StaysNull()
    {
        var errorLog = ErrorLog.Create(_projectId, "msg", "", "", "", "", "", null, null, null, null, _now);
        Assert.Null(errorLog.QueryString);
    }
}
