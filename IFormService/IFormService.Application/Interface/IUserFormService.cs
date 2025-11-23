using IFormService.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace IFormService.Application.Interface;

public interface IUserFormService
{
    /// <summary>
    /// Gets all forms
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<FormListDto>> GetAllForms();
    /// <summary>
    /// Download File 
    /// </summary>
    /// <param name="file">file path</param>
    /// <returns></returns>
    Task<StreamContent> DownloadFile(string file);
    /// <summary>
    /// Gets by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IEnumerable<FormListDto>> SearchInfo(string keyword);
    /// <summary>
    /// Creates the form and save in persistence
    /// </summary>
    /// <param name="userFormDto"></param>
    /// <returns></returns>
    Task<bool> CreateForm(UserFormDto userFormDto);
    
    /// <summary>
    /// Upload the file
    /// </summary>
    /// <param name="fileStream"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<UploadResultsDto> UploadFileAsync(IFormFile fileStream);
}