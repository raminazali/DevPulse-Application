namespace DevPulse.Core.Abstractions.Configuration;

/// <summary>
/// Configuration settings for S3 storage.
/// </summary>
public class S3Settings
{
    /// <summary>
    /// S3 bucket name for storing files.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// AWS region endpoint (e.g., us-east-1, eu-west-1).
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// URL template for constructing S3 URLs. Format: https://bucket.s3.region.amazonaws.com/{0}/{1}
    /// Where {0} is bucket name and {1} is object key.
    /// </summary>
    public string UrlTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Request timeout in seconds for S3 operations.
    /// </summary>
    public int RequestTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Validates the S3 settings configuration.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BucketName))
            throw new InvalidOperationException("S3Settings.BucketName is required");

        if (string.IsNullOrWhiteSpace(Region))
            throw new InvalidOperationException("S3Settings.Region is required");

        if (string.IsNullOrWhiteSpace(UrlTemplate))
            throw new InvalidOperationException("S3Settings.UrlTemplate is required");

        if (RequestTimeoutSeconds <= 0)
            throw new InvalidOperationException("S3Settings.RequestTimeoutSeconds must be greater than 0");
    }
}
