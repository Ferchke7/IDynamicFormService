using System.Text.Json;
using IFormService.Application;
using IFormService.Application.Dtos;
using IFormService.Application.Interface;
using IFormService.Domain.Enums;
using IFormService.Storage.SqLite.Entity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IFormService.Test;

public class UserFormServiceTests : IDisposable
{
    private readonly DbSqlite _dbContext;
    private readonly UserFormService _service;
    private readonly SqliteConnection _connection;

    public UserFormServiceTests()
    {
        SQLitePCL.Batteries.Init();
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DbSqlite>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new DbSqlite(options);
        _dbContext.Database.EnsureCreated();

        _dbContext.Database.ExecuteSqlRaw(@"
            CREATE VIRTUAL TABLE IF NOT EXISTS SubmissionFts USING fts5(
                Id UNINDEXED, 
                FormData
            );
            CREATE TRIGGER IF NOT EXISTS insert_SubmissionFts AFTER INSERT ON Submissions
            BEGIN
                INSERT INTO SubmissionFts(Id, FormData) VALUES (new.Id, new.FormData);
            END;
            CREATE TRIGGER IF NOT EXISTS delete_SubmissionFts AFTER DELETE ON Submissions
            BEGIN
                DELETE FROM SubmissionFts WHERE Id = old.Id;
            END;
            CREATE TRIGGER IF NOT EXISTS update_SubmissionFts AFTER UPDATE ON Submissions
            BEGIN
                UPDATE SubmissionFts SET FormData = new.FormData WHERE Id = new.Id;
            END;
        ");

        var mockSaveFileService = new MockSaveFileService();
        _service = new UserFormService(_dbContext, mockSaveFileService);
    }

    [Fact]
    public async Task CreateForm_ShouldCreateSubmission()
    {
        var formDto = new UserFormDto
        {
            Form = JsonDocument.Parse("{\"name\": \"Test User\", \"email\": \"test@example.com\"}"),
            AttachmentIds = new List<Guid>()
        };

        var result = await _service.CreateForm(formDto);

        Assert.True(result);
        var submissions = await _dbContext.Submissions.ToListAsync();
        Assert.Single(submissions);
        Assert.Equal("Test User", submissions[0].FormData.RootElement.GetProperty("name").GetString());
    }

    [Fact]
    public async Task CreateForm_WithAttachments_ShouldLinkAttachments()
    {
        var attachment = new Attachment
        {
            AttachmentId = Guid.NewGuid(),
            StoragePath = "test.pdf",
            StorageType = StorageType.LocalFileSystem
        };
        _dbContext.Attachments.Add(attachment);
        await _dbContext.SaveChangesAsync();

        var formDto = new UserFormDto
        {
            Form = JsonDocument.Parse("{\"title\": \"Test Form\"}"),
            AttachmentIds = new List<Guid> { attachment.AttachmentId }
        };

        var result = await _service.CreateForm(formDto);

        Assert.True(result);
        var submission = await _dbContext.Submissions.Include(s => s.Attachments).FirstAsync();
        Assert.Single(submission.Attachments);
        Assert.Equal(attachment.AttachmentId, submission.Attachments[0].AttachmentId);
    }

    [Fact]
    public async Task GetAllForms_ShouldReturnAllSubmissions()
    {
        var form1 = new Submission
        {
            Id = Guid.NewGuid(),
            FormData = JsonDocument.Parse("{\"name\": \"Form 1\"}"),
        };
        var form2 = new Submission
        {
            Id = Guid.NewGuid(),
            FormData = JsonDocument.Parse("{\"name\": \"Form 2\"}"),
        };

        _dbContext.Submissions.AddRange(form1, form2);
        await _dbContext.SaveChangesAsync();

        var results = await _service.GetAllForms();

        Assert.Equal(2, results.Count());
    }

    [Fact]
    public async Task SearchInfo_ShouldReturnMatchingForms()
    {
        var form1 = new Submission
        {
            Id = Guid.NewGuid(),
            FormData = JsonDocument.Parse("{\"name\": \"John Doe\", \"city\": \"New York\"}"),
        };
        var form2 = new Submission
        {
            Id = Guid.NewGuid(),
            FormData = JsonDocument.Parse("{\"name\": \"Jane Smith\", \"city\": \"London\"}"),
        };
        var form3 = new Submission
        {
            Id = Guid.NewGuid(),
            FormData = JsonDocument.Parse("{\"name\": \"Bob Jones\", \"city\": \"New York\"}"),
        };

        _dbContext.Submissions.AddRange(form1, form2, form3);
        await _dbContext.SaveChangesAsync();

        var results = await _service.SearchInfo("New York");

        Assert.Equal(2, results.Count());
        Assert.Contains(results, r => r.JsonDocument.RootElement.GetProperty("name").GetString() == "John Doe");
        Assert.Contains(results, r => r.JsonDocument.RootElement.GetProperty("name").GetString() == "Bob Jones");
    }

    [Fact]
    public async Task SearchInfo_ShouldReturnEmpty_WhenNoMatch()
    {
        var form1 = new Submission
        {
            Id = Guid.NewGuid(),
            FormData = JsonDocument.Parse("{\"name\": \"John Doe\", \"city\": \"New York\"}"),
        };
        _dbContext.Submissions.Add(form1);
        await _dbContext.SaveChangesAsync();

        var results = await _service.SearchInfo("Paris");

        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchInfo_WithSpecialCharacters_ShouldSanitize()
    {
        var form = new Submission
        {
            Id = Guid.NewGuid(),
            FormData = JsonDocument.Parse("{\"description\": \"Test content\"}"),
        };
        _dbContext.Submissions.Add(form);
        await _dbContext.SaveChangesAsync();

        var results = await _service.SearchInfo("Test*^\"");

        Assert.NotNull(results);
    }

    [Fact]
    public void CreateForm_WithNullFormData_ShouldThrowException()
    {
        var formDto = new UserFormDto
        {
            Form = null!,
            AttachmentIds = new List<Guid>()
        };

        Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateForm(formDto));
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
        _dbContext.Dispose();
    }
}

public class MockSaveFileService : ISaveAttachment
{
    public Task<StreamContent> DownloadFile(string file) => throw new NotImplementedException();
    public Task<UploadResultsDto> UploadFile(Stream fileStream, string fileName) => throw new NotImplementedException();
}
