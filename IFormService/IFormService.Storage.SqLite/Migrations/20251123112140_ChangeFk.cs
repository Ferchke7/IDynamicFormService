using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IFormService.Storage.SqLite.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create FTS5 virtual table
            migrationBuilder.Sql(@"
                CREATE VIRTUAL TABLE IF NOT EXISTS SubmissionFts USING fts5(
                    Id UNINDEXED, 
                    FormData
                );
            ");

            // Trigger on INSERT
            migrationBuilder.Sql(@"
                CREATE TRIGGER IF NOT EXISTS insert_SubmissionFts AFTER INSERT ON Submissions
                BEGIN
                    INSERT INTO SubmissionFts(Id, FormData) VALUES (new.Id, new.FormData);
                END;
            ");

            // Trigger on DELETE
            migrationBuilder.Sql(@"
                CREATE TRIGGER IF NOT EXISTS delete_SubmissionFts AFTER DELETE ON Submissions
                BEGIN
                    DELETE FROM SubmissionFts WHERE Id = old.Id;
                END;
            ");

            // Trigger on UPDATE
            migrationBuilder.Sql(@"
                CREATE TRIGGER IF NOT EXISTS update_SubmissionFts AFTER UPDATE ON Submissions
                BEGIN
                    UPDATE SubmissionFts SET FormData = new.FormData WHERE Id = new.Id;
                END;
            ");

            // Populate existing data
            migrationBuilder.Sql(@"
                INSERT INTO SubmissionFts(Id, FormData) SELECT Id, FormData FROM Submissions;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS insert_SubmissionFts;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS delete_SubmissionFts;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS update_SubmissionFts;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS SubmissionFts;");
        }
    }
}
