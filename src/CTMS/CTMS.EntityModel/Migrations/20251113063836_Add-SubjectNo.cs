using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CTMS.EntityModel.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubjectCode",
                table: "OperationHistoryTrace",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectCode",
                table: "OperationHistoryTrace");
        }
    }
}
