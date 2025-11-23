using IFormService.Domain.Enums;

namespace IFormService.Storage.SqLite.Entity;

public class Attachment
{
    public Guid AttachmentId { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public StorageType StorageType { get; set; }
    public Guid? SubmissionId { get; set; }
    public Submission? Submission { get; set; }
}