using Microsoft.AspNetCore.Http;

namespace IFormService.Application.Dtos;

public class AttachmentDto
{
    public IFormFile file { get; set; }
}