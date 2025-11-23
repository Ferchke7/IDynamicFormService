using IFormService.Application.Dtos;

namespace IFormService.Application.Validation;

public static class ValidationExtensions
{
    private static readonly HashSet<string> AllowedExtensions = new()
    {
        ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip"
    };

    private const long MaxFileSizeBytes = 100 * 1024 * 1024;

    public static void ValidateFileUpload(string fileName, long fileSize)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(extension))
            throw new ArgumentException("File must have an extension");

        if (!AllowedExtensions.Contains(extension))
            throw new ArgumentException($"File type '{extension}' is not allowed");

        if (fileSize > MaxFileSizeBytes)
            throw new ArgumentException($"File size exceeds maximum allowed size of {MaxFileSizeBytes / 1024 / 1024}MB");

        if (fileSize <= 0)
            throw new ArgumentException("File is empty");
    }

    public static void ValidateFormData(UserFormDto formDto)
    {
        if (formDto?.Form == null)
            throw new ArgumentNullException(nameof(formDto.Form));

        if (formDto.AttachmentIds == null)
            throw new ArgumentNullException(nameof(formDto.AttachmentIds));
    }

    public static string SanitizeFtsQuery(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return string.Empty;

        return keyword
            .Replace("\"", "\"\"")
            .Replace("*", "")
            .Replace("^", "")
            .Trim();
    }
}
