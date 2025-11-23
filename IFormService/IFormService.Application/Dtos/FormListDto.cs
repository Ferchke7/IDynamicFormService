using System.Text.Json;

namespace IFormService.Application.Dtos;

public class FormListDto
{
    public List<AttachmentListDto> AttachmentList { get; set; }
    public JsonDocument JsonDocument { get; set; }
}