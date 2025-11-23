using IFormService.Application.Dtos;
using IFormService.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace IFormService.Api.Controllers;

public static class FormEndpoints
{
    public static void AddFormEndpoints(this WebApplication app)
    {
        app.MapPost("/api/submissions", SubmitForm);
        app.MapGet("/api/submissions", ListForms);
        app.MapPost("/api/attachments", UploadFile).DisableAntiforgery();
        app.MapGet("/api/attachments/{fileName}", DownloadFile);
        app.MapGet("/api/submissions/search", SearchSubmissions);
    }

    private static async Task<IResult> SubmitForm(UserFormDto userFormDto, IUserFormService userFormService)
    {
        var result = await userFormService.CreateForm(userFormDto);
        return result 
            ? Results.Created("/api/submissions", new { success = true }) 
            : Results.BadRequest(new { error = "Failed to create submission" });
    }

    private static async Task<IResult> ListForms(IUserFormService userFormService)
    {
        var forms = await userFormService.GetAllForms();
        return Results.Ok(forms);
    }

    private static async Task<IResult> UploadFile([FromForm] AttachmentDto attachmentDto, IUserFormService userFormService)
    {
        var result = await userFormService.UploadFileAsync(attachmentDto.file);
        return Results.Ok(result);
    }

    private static async Task<IResult> DownloadFile(string fileName, IUserFormService userFormService)
    {
        var stream = await userFormService.DownloadFile(fileName);

        if (stream == null)
            return Results.NotFound(new { error = "File not found" });

        return Results.File(
            fileStream: stream.ReadAsStream(),
            contentType: "application/octet-stream",
            fileDownloadName: Path.GetFileName(fileName)
        );
    }

    private static async Task<IResult> SearchSubmissions(string keyword, IUserFormService userFormService)
    {
        var results = await userFormService.SearchInfo(keyword ?? string.Empty);
        return Results.Ok(results);
    }
}