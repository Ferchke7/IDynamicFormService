using System.Text.Json;

namespace IFormService.Application.Dtos;

public class UserFormDto
{
   /// <summary>
   /// Form data
   /// </summary>
   public JsonDocument Form { get; set; }
   
   /// <summary>
   /// Attachment IDs
   /// </summary>
   public List<Guid> AttachmentIds { get; set; }
   
}
