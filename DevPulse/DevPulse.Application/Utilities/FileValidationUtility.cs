using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace DevPulse.Application.Utilities;

/// <summary>
/// Utility class for file operations and validation.
/// </summary>
public static class FileValidationUtility
{
    private static readonly string[] AllowedImageExtensions = { ".png", ".jpg", ".jpeg" };
    private static readonly string[] AllowedImageMimeTypes = { "image/png", "image/jpeg" };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB

    /// <summary>
    /// Validates if the uploaded file is a supported image format.
    /// </summary>
    public static bool IsValidImageFormat(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(extension))
            return false;

        if (!AllowedImageMimeTypes.Contains(file.ContentType))
            return false;

        return true;
    }

    /// <summary>
    /// Validates if the file size is within acceptable limits.
    /// </summary>
    public static bool IsValidFileSize(IFormFile file)
    {
        return file != null && file.Length > 0 && file.Length <= MaxFileSizeBytes;
    }

    /// <summary>
    /// Calculates the MD5 hash of the file content.
    /// </summary>
    public static string CalculateMD5Hash(Stream stream)
    {
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }

    /// <summary>
    /// Generates a unique S3 object key for the screenshot.
    /// Format: screenshots/{errorEventId}/{timestamp}-{filename}
    /// </summary>
    public static string GenerateS3ObjectKey(Guid errorEventId, string fileName)
    {
        var timestamp = DateTime.UtcNow.Ticks;
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return $"screenshots/{errorEventId}/{timestamp}{extension}";
    }

    /// <summary>
    /// Validates screenshot file for upload.
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateScreenshot(IFormFile? file)
    {
        if (file == null)
            return (true, string.Empty); // Screenshot is optional

        if (!IsValidImageFormat(file))
            return (false, "Invalid image format. Only PNG and JPEG images are allowed.");

        if (!IsValidFileSize(file))
            return (false, $"File size exceeds maximum limit of 5MB.");

        return (true, string.Empty);
    }
}
