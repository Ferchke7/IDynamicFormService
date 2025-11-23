namespace IFormService.Application.Dtos;

public class UploadResultsDto
{
    public Guid guid { get; set; }
    public string filePath { get; set; }

    public UploadResultsDto(Guid guid, string filePath)
    {
        this.guid = guid;
        this.filePath = filePath;
    }
}