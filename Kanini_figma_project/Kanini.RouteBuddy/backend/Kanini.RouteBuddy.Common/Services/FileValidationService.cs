using Microsoft.AspNetCore.Http;

namespace Kanini.RouteBuddy.Common.Services;

public static class FileValidationService
{
    private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public static bool IsValidFile(IFormFile file, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (file == null || file.Length == 0)
        {
            errorMessage = "File is required";
            return false;
        }

        if (file.Length > MaxFileSize)
        {
            errorMessage = "File must be under 5MB";
            return false;
        }

        if (string.IsNullOrWhiteSpace(file.FileName))
        {
            errorMessage = "File name is required";
            return false;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            errorMessage = "File must be PDF, JPG, JPEG, or PNG format";
            return false;
        }

        // Check for suspicious file names
        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(fileName) || fileName.Length < 1)
        {
            errorMessage = "Invalid file name";
            return false;
        }

        // Basic content type validation
        var allowedContentTypes = new[] { "application/pdf", "image/jpeg", "image/jpg", "image/png" };
        if (!string.IsNullOrEmpty(file.ContentType) && !allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            errorMessage = "Invalid file content type";
            return false;
        }

        return true;
    }

    public static async Task<string> SaveFileAsync(IFormFile file, string directory, string fileName)
    {
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", directory);
        Directory.CreateDirectory(uploadsPath);

        var extension = Path.GetExtension(file.FileName);
        var sanitizedFileName = SanitizeFileName(fileName);
        var fullFileName = $"{sanitizedFileName}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
        var filePath = Path.Combine(uploadsPath, fullFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Path.Combine("uploads", directory, fullFileName).Replace("\\", "/");
    }

    private static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return "file";

        // Remove invalid characters and replace with underscore
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
        
        // Remove any remaining problematic characters
        sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, @"[^a-zA-Z0-9_-]", "_");
        
        return sanitized.Length > 50 ? sanitized.Substring(0, 50) : sanitized;
    }

    public static async Task<bool> IsValidDocumentContentAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) return false;

        try
        {
            using var stream = file.OpenReadStream();
            var buffer = new byte[8];
            await stream.ReadAsync(buffer, 0, 8);
            stream.Position = 0;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            return extension switch
            {
                ".pdf" => buffer[0] == 0x25 && buffer[1] == 0x50 && buffer[2] == 0x44 && buffer[3] == 0x46, // %PDF
                ".jpg" or ".jpeg" => buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF, // JPEG
                ".png" => buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47, // PNG
                _ => false
            };
        }
        catch
        {
            return false;
        }
    }
}