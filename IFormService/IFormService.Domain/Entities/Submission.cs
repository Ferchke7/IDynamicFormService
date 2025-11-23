using System.Text.Json;

namespace IFormService.Storage.SqLite.Entity;

public class Submission
{
    public Guid Id { get; set; }
    
    public JsonDocument FormData { get; set; } = JsonDocument.Parse("{}");
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public List<Attachment> Attachments { get; set; } = new();
    
    public Submission() {}

    public Submission(JsonDocument formData, List<Attachment> attachments)
    {
        Id = Guid.NewGuid();
        FormData = formData;
        Attachments = attachments;
        SubmittedAt = DateTime.UtcNow;
    }
}