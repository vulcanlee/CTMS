using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CTMS.EntityModel.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationHistoryTrace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperationHistoryTrace",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    User = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationHistoryTrace", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationHistoryTrace_CreateAt",
                table: "OperationHistoryTrace",
                column: "CreateAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationHistoryTrace");
        }
    }
}
