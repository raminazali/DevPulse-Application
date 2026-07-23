using DevPulse.Core.Exceptions;

namespace DevPulse.Core.Test.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void DevPulseException_SetsProperties()
    {
        var ex = new TestException("test message", "TEST_CODE", "Test Title");

        Assert.Equal("test message", ex.Message);
        Assert.Equal("TEST_CODE", ex.Code);
        Assert.Equal("Test Title", ex.Title);
    }

    [Fact]
    public void DevPulseException_CanHaveInnerException()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new TestException("outer", "CODE", "Title", inner);

        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void NotFoundException_DefaultCode()
    {
        var ex = new NotFoundException("not found");
        Assert.Equal("not found", ex.Message);
        Assert.Equal("NOT_FOUND", ex.Code);
        Assert.Equal("Resource not found", ex.Title);
    }

    [Fact]
    public void NotFoundException_CustomCode()
    {
        var ex = new NotFoundException("not found", "CUSTOM_NOT_FOUND");
        Assert.Equal("CUSTOM_NOT_FOUND", ex.Code);
    }

    [Fact]
    public void UnauthorizedException_DefaultCode()
    {
        var ex = new UnauthorizedException("unauthorized");
        Assert.Equal("unauthorized", ex.Message);
        Assert.Equal("UNAUTHORIZED", ex.Code);
        Assert.Equal("Authentication failed", ex.Title);
    }

    [Fact]
    public void UnauthorizedException_CustomCode()
    {
        var ex = new UnauthorizedException("unauthorized", "AUTH_FAILED");
        Assert.Equal("AUTH_FAILED", ex.Code);
    }

    [Fact]
    public void ForbiddenException_DefaultCode()
    {
        var ex = new ForbiddenException("forbidden");
        Assert.Equal("forbidden", ex.Message);
        Assert.Equal("FORBIDDEN", ex.Code);
        Assert.Equal("Access denied", ex.Title);
    }

    [Fact]
    public void ForbiddenException_CustomCode()
    {
        var ex = new ForbiddenException("forbidden", "ACCESS_DENIED");
        Assert.Equal("ACCESS_DENIED", ex.Code);
    }

    [Fact]
    public void BusinessRuleException_DefaultCode()
    {
        var ex = new BusinessRuleException("rule violated");
        Assert.Equal("rule violated", ex.Message);
        Assert.Equal("BUSINESS_RULE_VIOLATION", ex.Code);
        Assert.Equal("Business rule violation", ex.Title);
    }

    [Fact]
    public void BusinessRuleException_CustomCode()
    {
        var ex = new BusinessRuleException("rule", "MY_RULE");
        Assert.Equal("MY_RULE", ex.Code);
    }

    [Fact]
    public void ValidationException_DefaultCode()
    {
        var ex = new ValidationException("validation failed");
        Assert.Equal("validation failed", ex.Message);
        Assert.Equal("VALIDATION_ERROR", ex.Code);
        Assert.Equal("Validation failed", ex.Title);
    }

    [Fact]
    public void ValidationException_EmptyErrorsByDefault()
    {
        var ex = new ValidationException("validation failed");
        Assert.NotNull(ex.Errors);
        Assert.Empty(ex.Errors);
    }

    [Fact]
    public void ValidationException_WithErrors()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required" } },
            { "Name", new[] { "Name is too short", "Name is required" } }
        };
        var ex = new ValidationException("validation failed", errors);

        Assert.Equal(2, ex.Errors.Count);
        Assert.Equal(new[] { "Email is required" }, ex.Errors["Email"]);
        Assert.Equal(new[] { "Name is too short", "Name is required" }, ex.Errors["Name"]);
    }

    [Fact]
    public void ValidationException_CustomCode()
    {
        var ex = new ValidationException("failed", code: "INVALID_INPUT");
        Assert.Equal("INVALID_INPUT", ex.Code);
    }

    private sealed class TestException : DevPulseException
    {
        public TestException(string message, string code, string title, Exception? inner = null)
            : base(message, code, title, inner) { }
    }
}
