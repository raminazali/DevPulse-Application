namespace DevPulse.Core.Abstractions.Services;

/// <summary>
/// Interface for S3 storage service operations.
/// </summary>
public interface IS3StorageService
{
    /// <summary>
    /// Uploads a file to S3 storage.
    /// </summary>
    Task<string> UploadFileAsync(
        string bucketName,
        string objectKey,
        byte[] fileData,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from S3 storage.
    /// </summary>
    Task DeleteFileAsync(
        string bucketName,
        string objectKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a file exists in S3 storage.
    /// </summary>
    Task<bool> FileExistsAsync(
        string bucketName,
        string objectKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from S3 storage.
    /// </summary>
    Task<Stream> GetFileAsync(
        string bucketName,
        string objectKey,
        CancellationToken cancellationToken = default);
}
