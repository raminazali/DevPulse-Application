using DevPulse.Core.ValueObjects;

namespace DevPulse.Core.Test.ValueObjects;

public class ErrorFingerprintTests
{
    [Fact]
    public void Create_ReturnsString()
    {
        var result = ErrorFingerprint.Create("message", "stack", "url");
        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public void Create_SameInputs_ReturnsSameHash()
    {
        var hash1 = ErrorFingerprint.Create("error occurred", "at Foo()", "http://example.com");
        var hash2 = ErrorFingerprint.Create("error occurred", "at Foo()", "http://example.com");

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Create_DifferentMessage_ReturnsDifferentHash()
    {
        var hash1 = ErrorFingerprint.Create("error A", "at Foo()", "http://example.com");
        var hash2 = ErrorFingerprint.Create("error B", "at Foo()", "http://example.com");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Create_DifferentStackTrace_ReturnsDifferentHash()
    {
        var hash1 = ErrorFingerprint.Create("error", "at Foo()", "http://example.com");
        var hash2 = ErrorFingerprint.Create("error", "at Bar()", "http://example.com");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Create_DifferentUrl_ReturnsDifferentHash()
    {
        var hash1 = ErrorFingerprint.Create("error", "stack", "http://site1.com");
        var hash2 = ErrorFingerprint.Create("error", "stack", "http://site2.com");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Create_WithEmptyStrings_DoesNotThrow()
    {
        var ex = Record.Exception(() => ErrorFingerprint.Create("", "", ""));
        Assert.Null(ex);
    }
}
