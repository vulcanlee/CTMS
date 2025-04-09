using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CTMS.EntityModel.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    醫院 = table.Column<string>(type: "TEXT", nullable: false),
                    癌別 = table.Column<string>(type: "TEXT", nullable: false),
                    JsonData = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.Id);
                });

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
                name: "Athlete",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    FilesData = table.Column<string>(type: "TEXT", nullable: false),
                    ExcelData = table.Column<string>(type: "TEXT", nullable: false),
                    ExamineTime = table.Column<string>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Athlete", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Athlete_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MyUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Account = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Salt = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<bool>(type: "INTEGER", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    RoleJson = table.Column<string>(type: "TEXT", nullable: false),
                    RoleViewId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyUser_RoleView_RoleViewId",
                        column: x => x.RoleViewId,
                        principalTable: "RoleView",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "Examine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    ExamineTime = table.Column<string>(type: "TEXT", nullable: false),
                    FilesData = table.Column<string>(type: "TEXT", nullable: false),
                    ExcelData = table.Column<string>(type: "TEXT", nullable: false),
                    AthleteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Examine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Examine_Athlete_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athlete",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Athlete_ProjectId",
                table: "Athlete",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Examine_AthleteId",
                table: "Examine",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_MyUser_RoleViewId",
                table: "MyUser",
                column: "RoleViewId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Examine");

            migrationBuilder.DropTable(
                name: "MyUser");

            migrationBuilder.DropTable(
                name: "Patient");

            migrationBuilder.DropTable(
                name: "ProjectRoleView");

            migrationBuilder.DropTable(
                name: "RoleViewProject");

            migrationBuilder.DropTable(
                name: "Athlete");

            migrationBuilder.DropTable(
                name: "RoleView");

            migrationBuilder.DropTable(
                name: "Project");
        }
    }
}
