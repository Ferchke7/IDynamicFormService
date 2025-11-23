using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IFormService.Storage.SqLite.Migrations
{
    /// <inheritdoc />
    public partial class FixSubmissionIdTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Submissions_SubmittionId",
                table: "Attachments");

            migrationBuilder.RenameColumn(
                name: "SubmittionId",
                table: "Attachments",
                newName: "SubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachments_SubmittionId",
                table: "Attachments",
                newName: "IX_Attachments_SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Submissions_SubmissionId",
                table: "Attachments",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Submissions_SubmissionId",
                table: "Attachments");

            migrationBuilder.RenameColumn(
                name: "SubmissionId",
                table: "Attachments",
                newName: "SubmittionId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachments_SubmissionId",
                table: "Attachments",
                newName: "IX_Attachments_SubmittionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Submissions_SubmittionId",
                table: "Attachments",
                column: "SubmittionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
