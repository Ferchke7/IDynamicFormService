using IFormService.Application.Dtos;
using IFormService.Application.Interface;
using IFormService.Application.Validation;

namespace IFormService.File.LocalStorage.Service;

public class FileService : ISaveAttachment
{
    private const string UploadDirectory = "uploads";

    public async Task<UploadResultsDto> UploadFile(Stream incomingStream, string originalFileName)
    {
        EnsureDirectoryExists();
        
        var fileId = Guid.NewGuid();
        var extension = Path.GetExtension(originalFileName);
        var fileName = $"{fileId}{extension}";
        var filePath = Path.Combine(UploadDirectory, fileName);

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await incomingStream.CopyToAsync(fileStream);
        }

        return new UploadResultsDto(fileId, filePath);
    }

    public async Task<StreamContent> DownloadFile(string fileName)
    {
        if (!System.IO.File.Exists(fileName))
            throw new FileNotFoundException("File not found", fileName);

        var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        return new StreamContent(fileStream);
    }
    
    private static void EnsureDirectoryExists()
    {
        if (!Directory.Exists(UploadDirectory))
            Directory.CreateDirectory(UploadDirectory);
    }
}