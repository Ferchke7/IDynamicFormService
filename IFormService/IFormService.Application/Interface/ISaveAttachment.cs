using IFormService.Application.Dtos;

namespace IFormService.Application.Interface;

public interface ISaveAttachment
{
    /// <summary>
    /// Uploads the file in wwwroot in the file system
    /// </summary>
    /// <param name="fileStream">stream of the file</param>
    /// <param name="fileName">filename</param>
    /// <returns>UploadResultDto.cs with Guid and file path, file path for future in UI feature intentionally exposed</returns>
    Task<UploadResultsDto> UploadFile(Stream fileStream, string fileName);
    
    Task<StreamContent> DownloadFile(string fileName);
}