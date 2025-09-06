using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CTMS.EntityModel.Migrations
{
    /// <inheritdoc />
    public partial class AddAIProcessing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AI處理",
                table: "Patient",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AI處理",
                table: "Patient");
        }
    }
}
