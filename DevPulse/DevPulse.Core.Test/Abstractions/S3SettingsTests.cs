using DevPulse.Core.Abstractions.Configuration;

namespace DevPulse.Core.Test.Abstractions;

public class S3SettingsTests
{
    private static S3Settings CreateValid() => new()
    {
        BucketName = "my-bucket",
        Region = "us-east-1",
        UrlTemplate = "https://{0}.s3.amazonaws.com/{1}",
        RequestTimeoutSeconds = 30
    };

    [Fact]
    public void Validate_WithValidSettings_DoesNotThrow()
    {
        var settings = CreateValid();
        var ex = Record.Exception(() => settings.Validate());
        Assert.Null(ex);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyBucketName_Throws(string bucketName)
    {
        var settings = CreateValid();
        settings.BucketName = bucketName;

        var ex = Assert.Throws<InvalidOperationException>(() => settings.Validate());
        Assert.Contains("BucketName", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyRegion_Throws(string region)
    {
        var settings = CreateValid();
        settings.Region = region;

        var ex = Assert.Throws<InvalidOperationException>(() => settings.Validate());
        Assert.Contains("Region", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyUrlTemplate_Throws(string urlTemplate)
    {
        var settings = CreateValid();
        settings.UrlTemplate = urlTemplate;

        var ex = Assert.Throws<InvalidOperationException>(() => settings.Validate());
        Assert.Contains("UrlTemplate", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_InvalidTimeout_Throws(int timeout)
    {
        var settings = CreateValid();
        settings.RequestTimeoutSeconds = timeout;

        var ex = Assert.Throws<InvalidOperationException>(() => settings.Validate());
        Assert.Contains("RequestTimeoutSeconds", ex.Message);
    }

    [Fact]
    public void Validate_DefaultInstance_HasDefaultValues()
    {
        var settings = new S3Settings();
        Assert.Equal(string.Empty, settings.BucketName);
        Assert.Equal(string.Empty, settings.Region);
        Assert.Equal(string.Empty, settings.UrlTemplate);
        Assert.Equal(30, settings.RequestTimeoutSeconds);
    }
}
