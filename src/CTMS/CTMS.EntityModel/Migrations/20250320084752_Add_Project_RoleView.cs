using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CTMS.EntityModel.Migrations
{
    /// <inheritdoc />
    public partial class Add_Project_RoleView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleViewId",
                table: "MyUser",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Athlete",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleView",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TabViewJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleView", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRoleView",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleViewId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRoleView", x => new { x.ProjectId, x.RoleViewId });
                    table.ForeignKey(
                        name: "FK_ProjectRoleView_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectRoleView_RoleView_RoleViewId",
                        column: x => x.RoleViewId,
                        principalTable: "RoleView",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleViewProject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleViewId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleViewProject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleViewProject_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleViewProject_RoleView_RoleViewId",
                        column: x => x.RoleViewId,
                        principalTable: "RoleView",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MyUser_RoleViewId",
                table: "MyUser",
                column: "RoleViewId");

            migrationBuilder.CreateIndex(
                name: "IX_Athlete_ProjectId",
                table: "Athlete",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRoleView_RoleViewId",
                table: "ProjectRoleView",
                column: "RoleViewId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleViewProject_ProjectId",
                table: "RoleViewProject",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleViewProject_RoleViewId",
                table: "RoleViewProject",
                column: "RoleViewId");

            migrationBuilder.AddForeignKey(
                name: "FK_Athlete_Project_ProjectId",
                table: "Athlete",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MyUser_RoleView_RoleViewId",
                table: "MyUser",
                column: "RoleViewId",
                principalTable: "RoleView",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Athlete_Project_ProjectId",
                table: "Athlete");

            migrationBuilder.DropForeignKey(
                name: "FK_MyUser_RoleView_RoleViewId",
                table: "MyUser");

            migrationBuilder.DropTable(
                name: "ProjectRoleView");

            migrationBuilder.DropTable(
                name: "RoleViewProject");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "RoleView");

            migrationBuilder.DropIndex(
                name: "IX_MyUser_RoleViewId",
                table: "MyUser");

            migrationBuilder.DropIndex(
                name: "IX_Athlete_ProjectId",
                table: "Athlete");

            migrationBuilder.DropColumn(
                name: "RoleViewId",
                table: "MyUser");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Athlete");
        }
    }
}
