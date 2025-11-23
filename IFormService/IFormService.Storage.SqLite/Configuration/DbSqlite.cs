using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using IFormService.Storage.SqLite.Entity;

public class DbSqlite : DbContext
{
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Attachment> Attachments { get; set; }

    public DbSqlite(DbContextOptions<DbSqlite> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FormData)
                .HasConversion(
                    doc => doc.RootElement.GetRawText(),
                    json => JsonDocument.Parse(json, default)
                )
                .HasColumnType("TEXT");
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId);

            entity.HasOne(a => a.Submission)
                .WithMany(s => s.Attachments)
                .HasForeignKey(a => a.SubmissionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}