using Amazon.S3;
using Amazon.S3.Model;
using DevPulse.Core.Abstractions.Configuration;
using DevPulse.Core.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DevPulse.Infrastructure.Services
{
    /// <summary>
    /// Production-ready implementation of S3 storage service.
    /// Handles file upload, download, deletion, and metadata operations with proper error handling and logging.
    /// </summary>
    public class S3StorageService : IS3StorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IOptions<S3Settings> _s3Settings;
        private readonly ILogger<S3StorageService> _logger;

        /// <summary>
        /// Initializes a new instance of the S3StorageService class.
        /// </summary>
        /// <param name="s3Client">The AWS S3 client.</param>
        /// <param name="s3Settings">Configuration options for S3 settings.</param>
        /// <param name="logger">Logger for diagnostic and error logging.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when S3 settings are invalid.</exception>
        public S3StorageService(
            IAmazonS3 s3Client,
            IOptions<S3Settings> s3Settings,
            ILogger<S3StorageService> logger)
        {
            _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
            _s3Settings = s3Settings ?? throw new ArgumentNullException(nameof(s3Settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                _s3Settings.Value.Validate();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "S3 settings validation failed");
                throw;
            }
        }

        /// <summary>
        /// Uploads a file to S3 asynchronously with proper resource disposal and error handling.
        /// </summary>
        /// <param name="bucketName">The S3 bucket name.</param>
        /// <param name="objectKey">The object key (path) in S3.</param>
        /// <param name="fileData">The file data as byte array.</param>
        /// <param name="contentType">The MIME content type of the file.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>The S3 URL of the uploaded object.</returns>
        /// <exception cref="ArgumentException">Thrown when bucketName, objectKey, or contentType are null or whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when fileData is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when S3 upload fails.</exception>
        public async Task<string> UploadFileAsync(
            string bucketName,
            string objectKey,
            byte[] fileData,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            // Argument validation
            if (string.IsNullOrWhiteSpace(bucketName))
                throw new ArgumentException("Bucket name cannot be empty", nameof(bucketName));

            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key cannot be empty", nameof(objectKey));

            if (fileData == null)
                throw new ArgumentNullException(nameof(fileData));

            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content type cannot be empty", nameof(contentType));

            try
            {
                _logger.LogInformation(
                    "Uploading file to S3 - Bucket: {BucketName}, Key: {ObjectKey}, Size: {FileSize} bytes",
                    bucketName, objectKey, fileData.Length);

                using (var memoryStream = new MemoryStream(fileData))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = objectKey,
                        InputStream = memoryStream,
                        ContentType = contentType
                    };

                    var response = await _s3Client.PutObjectAsync(request, cancellationToken);

                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        var url = _s3Settings.Value.UrlTemplate.Replace("{0}", bucketName).Replace("{1}", objectKey);
                        _logger.LogInformation("File uploaded successfully - URL: {FileUrl}", url);
                        return url;
                    }
                    else
                    {
                        var errorMsg = $"S3 upload returned unexpected status code: {response.HttpStatusCode}";
                        _logger.LogError(errorMsg);
                        throw new InvalidOperationException(errorMsg);
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "File upload to S3 was cancelled - Bucket: {BucketName}, Key: {ObjectKey}", bucketName, objectKey);
                throw;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(
                    ex,
                    "AWS S3 error during file upload - Bucket: {BucketName}, Key: {ObjectKey}, ErrorCode: {ErrorCode}",
                    bucketName, objectKey, ex.ErrorCode);
                throw new InvalidOperationException($"Failed to upload file to S3: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error during file upload to S3 - Bucket: {BucketName}, Key: {ObjectKey}",
                    bucketName, objectKey);
                throw new InvalidOperationException($"An unexpected error occurred during file upload: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes a file from S3 asynchronously.
        /// </summary>
        /// <param name="bucketName">The S3 bucket name.</param>
        /// <param name="objectKey">The object key (path) in S3.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <exception cref="ArgumentException">Thrown when bucketName or objectKey are null or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown when S3 delete operation fails.</exception>
        public async Task DeleteFileAsync(
            string bucketName,
            string objectKey,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
                throw new ArgumentException("Bucket name cannot be empty", nameof(bucketName));

            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key cannot be empty", nameof(objectKey));

            try
            {
                _logger.LogInformation(
                    "Deleting file from S3 - Bucket: {BucketName}, Key: {ObjectKey}",
                    bucketName, objectKey);

                var request = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                var response = await _s3Client.DeleteObjectAsync(request, cancellationToken);

                if (response.HttpStatusCode == HttpStatusCode.OK || response.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    _logger.LogInformation("File deleted successfully from S3 - Bucket: {BucketName}, Key: {ObjectKey}", bucketName, objectKey);
                }
                else
                {
                    _logger.LogWarning(
                        "Unexpected status code when deleting file from S3 - Status: {StatusCode}, Bucket: {BucketName}, Key: {ObjectKey}",
                        response.HttpStatusCode, bucketName, objectKey);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "File deletion from S3 was cancelled - Bucket: {BucketName}, Key: {ObjectKey}", bucketName, objectKey);
                throw;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(
                    ex,
                    "AWS S3 error during file deletion - Bucket: {BucketName}, Key: {ObjectKey}, ErrorCode: {ErrorCode}",
                    bucketName, objectKey, ex.ErrorCode);
                throw new InvalidOperationException($"Failed to delete file from S3: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error during file deletion from S3 - Bucket: {BucketName}, Key: {ObjectKey}",
                    bucketName, objectKey);
                throw new InvalidOperationException($"An unexpected error occurred during file deletion: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if a file exists in S3 asynchronously.
        /// </summary>
        /// <param name="bucketName">The S3 bucket name.</param>
        /// <param name="objectKey">The object key (path) in S3.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>True if the object exists in S3; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Thrown when bucketName or objectKey are null or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown when S3 operation fails for reasons other than object not found.</exception>
        public async Task<bool> FileExistsAsync(
            string bucketName,
            string objectKey,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
                throw new ArgumentException("Bucket name cannot be empty", nameof(bucketName));

            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key cannot be empty", nameof(objectKey));

            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
                return true;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "File existence check in S3 was cancelled - Bucket: {BucketName}, Key: {ObjectKey}", bucketName, objectKey);
                throw;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogDebug("File does not exist in S3 - Bucket: {BucketName}, Key: {ObjectKey}", bucketName, objectKey);
                return false;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(
                    ex,
                    "AWS S3 error during file existence check - Bucket: {BucketName}, Key: {ObjectKey}, ErrorCode: {ErrorCode}",
                    bucketName, objectKey, ex.ErrorCode);
                throw new InvalidOperationException($"Failed to check file existence in S3: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error during file existence check in S3 - Bucket: {BucketName}, Key: {ObjectKey}",
                    bucketName, objectKey);
                throw new InvalidOperationException($"An unexpected error occurred during file existence check: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Retrieves a file from S3 as a stream asynchronously.
        /// </summary>
        /// <param name="bucketName">The S3 bucket name.</param>
        /// <param name="objectKey">The object key (path) in S3.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>A stream containing the file data. Caller is responsible for disposing the stream.</returns>
        /// <exception cref="ArgumentException">Thrown when bucketName or objectKey are null or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the object does not exist or retrieval fails.</exception>
        public async Task<Stream> GetFileAsync(
            string bucketName,
            string objectKey,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
                throw new ArgumentException("Bucket name cannot be empty", nameof(bucketName));

            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key cannot be empty", nameof(objectKey));

            try
            {
                _logger.LogInformation(
                    "Retrieving file from S3 - Bucket: {BucketName}, Key: {ObjectKey}",
                    bucketName, objectKey);

                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                var response = await _s3Client.GetObjectAsync(request, cancellationToken);

                if (response.ResponseStream == null)
                {
                    throw new InvalidOperationException("S3 response stream is null");
                }

                _logger.LogInformation(
                    "File retrieved successfully from S3 - Bucket: {BucketName}, Key: {ObjectKey}",
                    bucketName, objectKey);

                return response.ResponseStream;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "File retrieval from S3 was cancelled - Bucket: {BucketName}, Key: {ObjectKey}", bucketName, objectKey);
                throw;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("File not found in S3 - Bucket: {BucketName}, Key: {ObjectKey}", bucketName, objectKey);
                throw new InvalidOperationException($"File not found in S3: {objectKey}", ex);
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(
                    ex,
                    "AWS S3 error during file retrieval - Bucket: {BucketName}, Key: {ObjectKey}, ErrorCode: {ErrorCode}",
                    bucketName, objectKey, ex.ErrorCode);
                throw new InvalidOperationException($"Failed to retrieve file from S3: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error during file retrieval from S3 - Bucket: {BucketName}, Key: {ObjectKey}",
                    bucketName, objectKey);
                throw new InvalidOperationException($"An unexpected error occurred during file retrieval: {ex.Message}", ex);
            }
        }
    }
}