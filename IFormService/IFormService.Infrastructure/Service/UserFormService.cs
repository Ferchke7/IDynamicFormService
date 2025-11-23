using IFormService.Application.Dtos;
using IFormService.Application.Interface;
using IFormService.Application.Validation;
using IFormService.Domain.Enums;
using IFormService.Storage.SqLite.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IFormService.Application;

public class UserFormService(DbSqlite dbContext, ISaveAttachment saveFileService) : IUserFormService
{
    public async Task<IEnumerable<FormListDto>> GetAllForms()
    {
        var userForms = await dbContext.Submissions
            .Include(u => u.Attachments)
            .AsNoTracking()
            .ToListAsync();

        return userForms.Select(e => new FormListDto
        {
            JsonDocument = e.FormData,
            AttachmentList = e.Attachments.Select(u => new AttachmentListDto
            {
                Guid = u.AttachmentId,
                Path = u.StoragePath
            }).ToList()
        });
    }
    
    public async Task<StreamContent> DownloadFile(string file)
    {
        return await saveFileService.DownloadFile(file);
    }

    public async Task<IEnumerable<FormListDto>> SearchInfo(string keyword)
    {
        var sanitizedKeyword = ValidationExtensions.SanitizeFtsQuery(keyword);
        
        if (string.IsNullOrWhiteSpace(sanitizedKeyword))
            return await GetAllForms();

        var userForms = await dbContext.Submissions
            .FromSqlInterpolated($@"
                SELECT s.* 
                FROM Submissions AS s
                JOIN SubmissionFts AS f ON s.Id = f.Id
                WHERE f.FormData MATCH {sanitizedKeyword}
            ")
            .Include(u => u.Attachments)
            .AsNoTracking()
            .ToListAsync();

        return userForms.Select(e => new FormListDto
        {
            JsonDocument = e.FormData,
            AttachmentList = e.Attachments.Select(u => new AttachmentListDto
            {
                Guid = u.AttachmentId,
                Path = u.StoragePath
            }).ToList()
        });
    }
    
    public async Task<bool> CreateForm(UserFormDto userFormDto)
    {
        ValidationExtensions.ValidateFormData(userFormDto);

        var submission = new Submission
        {
            FormData = userFormDto.Form,
        };

        dbContext.Submissions.Add(submission);
        
        if (userFormDto.AttachmentIds.Any())
        {
            var attachments = await dbContext.Attachments
                .Where(a => userFormDto.AttachmentIds.Contains(a.AttachmentId))
                .ToListAsync();

            foreach (var attachment in attachments)
            {
                attachment.Submission = submission;
            }
        }
        
        var result = await dbContext.SaveChangesAsync();
        return result > 0;
    }
    
    public async Task<UploadResultsDto> UploadFileAsync(IFormFile file)
    {
        ValidationExtensions.ValidateFileUpload(file.FileName, file.Length);

        using var stream = file.OpenReadStream();
        var fileSavedResult = await saveFileService.UploadFile(stream, file.FileName);
        var saveToDb = await SaveAttachment(fileSavedResult);
        
        if (!saveToDb)
            throw new Exception("Failed to save attachment to database");
            
        return fileSavedResult;
    }

    private async Task<bool> SaveAttachment(UploadResultsDto fileSavedResult)
    {
        dbContext.Attachments.Add(new Attachment
        {
            AttachmentId = fileSavedResult.guid,
            StorageType = StorageType.LocalFileSystem,
            StoragePath = fileSavedResult.filePath,
            Submission = null,
            SubmissionId = null
        });

        return await dbContext.SaveChangesAsync() > 0;
    }
}
