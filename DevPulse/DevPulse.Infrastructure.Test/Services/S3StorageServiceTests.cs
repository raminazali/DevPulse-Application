using Amazon.S3;
using Amazon.S3.Model;
using DevPulse.Core.Abstractions.Configuration;
using DevPulse.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DevPulse.Infrastructure.Test.Services
{
    /// <summary>
    /// Unit tests for S3StorageService covering all CRUD operations and error scenarios.
    /// </summary>
    public class S3StorageServiceTests
    {
        private readonly Mock<IAmazonS3> _s3ClientMock;
        private readonly Mock<ILogger<S3StorageService>> _loggerMock;
        private readonly IOptions<S3Settings> _s3Settings;
        private readonly S3StorageService _s3StorageService;

        public S3StorageServiceTests()
        {
            _s3ClientMock = new Mock<IAmazonS3>();
            _loggerMock = new Mock<ILogger<S3StorageService>>();

            _s3Settings = Options.Create(new S3Settings
            {
                BucketName = "test-bucket",
                Region = "us-east-1",
                UrlTemplate = "https://{0}.s3.amazonaws.com/{1}",
                RequestTimeoutSeconds = 30
            });

            _s3StorageService = new S3StorageService(
                _s3ClientMock.Object,
                _s3Settings,
                _loggerMock.Object);
        }

        #region UploadFileAsync Tests

        [Fact]
        public async Task UploadFileAsync_WithValidData_ReturnsS3Url()
        {
            // Arrange
            const string bucketName = "test-bucket";
            const string objectKey = "files/test.txt";
            byte[] fileData = System.Text.Encoding.UTF8.GetBytes("test content");
            const string contentType = "text/plain";

            var putResponse = new PutObjectResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                ETag = "\"test-etag\""
            };

            _s3ClientMock
                .Setup(s => s.PutObjectAsync(
                    It.IsAny<PutObjectRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(putResponse);

            // Act
            var result = await _s3StorageService.UploadFileAsync(
                bucketName,
                objectKey,
                fileData,
                contentType,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(bucketName, result);
            Assert.Contains(objectKey, result);
            _s3ClientMock.Verify(s => s.PutObjectAsync(
                It.IsAny<PutObjectRequest>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UploadFileAsync_WithInvalidBucketName_ThrowsArgumentException(string bucketName)
        {
            // Arrange
            byte[] fileData = System.Text.Encoding.UTF8.GetBytes("test content");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _s3StorageService.UploadFileAsync(
                    bucketName,
                    "key",
                    fileData,
                    "text/plain"));
        }

        [Fact]
        public async Task UploadFileAsync_WithNullFileData_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _s3StorageService.UploadFileAsync(
                    "bucket",
                    "key",
                    null,
                    "text/plain"));
        }

        [Fact]
        public async Task UploadFileAsync_WithS3Exception_ThrowsInvalidOperationException()
        {
            // Arrange
            byte[] fileData = System.Text.Encoding.UTF8.GetBytes("test content");
            var s3Exception = new AmazonS3Exception("Access denied", new Exception());

            _s3ClientMock
                .Setup(s => s.PutObjectAsync(
                    It.IsAny<PutObjectRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(s3Exception);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _s3StorageService.UploadFileAsync(
                    "bucket",
                    "key",
                    fileData,
                    "text/plain"));

            Assert.NotNull(exception.InnerException);
        }

        [Fact]
        public async Task UploadFileAsync_WithCancellation_ThrowsOperationCanceledException()
        {
            // Arrange
            byte[] fileData = System.Text.Encoding.UTF8.GetBytes("test content");
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _s3ClientMock
                .Setup(s => s.PutObjectAsync(
                    It.IsAny<PutObjectRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _s3StorageService.UploadFileAsync(
                    "bucket",
                    "key",
                    fileData,
                    "text/plain",
                    cancellationTokenSource.Token));
        }

        #endregion

        #region DeleteFileAsync Tests

        [Fact]
        public async Task DeleteFileAsync_WithValidData_CompletesSuccessfully()
        {
            // Arrange
            const string bucketName = "test-bucket";
            const string objectKey = "files/test.txt";

            var deleteResponse = new DeleteObjectResponse
            {
                HttpStatusCode = HttpStatusCode.OK
            };

            _s3ClientMock
                .Setup(s => s.DeleteObjectAsync(
                    It.IsAny<DeleteObjectRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(deleteResponse);

            // Act
            await _s3StorageService.DeleteFileAsync(bucketName, objectKey);

            // Assert
            _s3ClientMock.Verify(s => s.DeleteObjectAsync(
                It.IsAny<DeleteObjectRequest>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task DeleteFileAsync_WithInvalidBucketName_ThrowsArgumentException(string bucketName)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _s3StorageService.DeleteFileAsync(bucketName, "key"));
        }

        [Fact]
        public async Task DeleteFileAsync_WithS3Exception_ThrowsInvalidOperationException()
        {
            // Arrange
            var s3Exception = new AmazonS3Exception("Access denied", new Exception());

            _s3ClientMock
                .Setup(s => s.DeleteObjectAsync(
                    It.IsAny<DeleteObjectRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(s3Exception);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _s3StorageService.DeleteFileAsync("bucket", "key"));

            Assert.NotNull(exception.InnerException);
        }

        #endregion

        #region FileExistsAsync Tests

        [Fact]
        public async Task FileExistsAsync_WhenFileExists_ReturnsTrue()
        {
            // Arrange
            const string bucketName = "test-bucket";
            const string objectKey = "files/test.txt";

            var metadataResponse = new GetObjectMetadataResponse
            {
                HttpStatusCode = HttpStatusCode.OK
            };

            _s3ClientMock
                .Setup(s => s.GetObjectMetadataAsync(
                    It.IsAny<GetObjectMetadataRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(metadataResponse);

            // Act
            var result = await _s3StorageService.FileExistsAsync(bucketName, objectKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task FileExistsAsync_WhenFileNotFound_ReturnsFalse()
        {
            // Arrange
            const string bucketName = "test-bucket";
            const string objectKey = "files/nonexistent.txt";

            var s3Exception = new AmazonS3Exception("Not found")
            {
                StatusCode = HttpStatusCode.NotFound
            };

            _s3ClientMock
                .Setup(s => s.GetObjectMetadataAsync(
                    It.IsAny<GetObjectMetadataRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(s3Exception);

            // Act
            var result = await _s3StorageService.FileExistsAsync(bucketName, objectKey);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task FileExistsAsync_WithS3Exception_ThrowsInvalidOperationException()
        {
            // Arrange
            var s3Exception = new AmazonS3Exception("Access denied")
            {
                StatusCode = HttpStatusCode.Forbidden
            };

            _s3ClientMock
                .Setup(s => s.GetObjectMetadataAsync(
                    It.IsAny<GetObjectMetadataRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(s3Exception);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _s3StorageService.FileExistsAsync("bucket", "key"));

            Assert.NotNull(exception.InnerException);
        }

        #endregion

        #region GetFileAsync Tests

        [Fact]
        public async Task GetFileAsync_WithValidData_ReturnsStream()
        {
            // Arrange
            const string bucketName = "test-bucket";
            const string objectKey = "files/test.txt";
            const string fileContent = "test file content";

            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
            var getResponse = new GetObjectResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                ResponseStream = memoryStream
            };

            _s3ClientMock
                .Setup(s => s.GetObjectAsync(
                    It.IsAny<GetObjectRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(getResponse);

            // Act
            var result = await _s3StorageService.GetFileAsync(bucketName, objectKey);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<Stream>(result);
        }

        [Fact]
        public async Task GetFileAsync_WhenFileNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            const string bucketName = "test-bucket";
            const string objectKey = "files/nonexistent.txt";

            var s3Exception = new AmazonS3Exception("Not found")
            {
                StatusCode = HttpStatusCode.NotFound
            };

            _s3ClientMock
                .Setup(s => s.GetObjectAsync(
                    It.IsAny<GetObjectRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(s3Exception);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _s3StorageService.GetFileAsync(bucketName, objectKey));

            Assert.Contains("not found", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetFileAsync_WithInvalidBucketName_ThrowsArgumentException(string bucketName)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _s3StorageService.GetFileAsync(bucketName, "key"));
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullS3Client_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new S3StorageService(null, _s3Settings, _loggerMock.Object));
        }

        [Fact]
        public void Constructor_WithNullS3Settings_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new S3StorageService(_s3ClientMock.Object, null, _loggerMock.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new S3StorageService(_s3ClientMock.Object, _s3Settings, null));
        }

        [Fact]
        public void Constructor_WithInvalidS3Settings_ThrowsInvalidOperationException()
        {
            // Arrange
            var invalidSettings = Options.Create(new S3Settings
            {
                BucketName = "", // Invalid: empty bucket name
                Region = "us-east-1",
                UrlTemplate = "https://{0}.s3.amazonaws.com/{1}"
            });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                new S3StorageService(_s3ClientMock.Object, invalidSettings, _loggerMock.Object));
        }

        #endregion
    }
}
