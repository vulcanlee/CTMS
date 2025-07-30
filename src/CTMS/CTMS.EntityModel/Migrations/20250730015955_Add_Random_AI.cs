using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CTMS.EntityModel.Migrations
{
    /// <inheritdoc />
    public partial class Add_Random_AI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AI評估",
                table: "Patient",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "組別",
                table: "Patient",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AI評估",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "組別",
                table: "Patient");
        }
    }
}
