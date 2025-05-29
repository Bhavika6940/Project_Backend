using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class CleanStart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserModel",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModel", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "CourseModel",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaUrl = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseModel", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_CourseModel_UserModel",
                        column: x => x.UserId,
                        principalTable: "UserModel",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "AssessmentModel",
                columns: table => new
                {
                    AssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Questions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxScore = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentModel", x => x.AssessmentId);
                    table.ForeignKey(
                        name: "FK_AssessmentModel_CourseModel",
                        column: x => x.CourseId,
                        principalTable: "CourseModel",
                        principalColumn: "CourseId");
                });

            migrationBuilder.CreateTable(
                name: "ResultModel",
                columns: table => new
                {
                    ResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    AttemptDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultModel", x => x.ResultId);
                    table.ForeignKey(
                        name: "FK_ResultModel_AssessmentModel",
                        column: x => x.AssessmentId,
                        principalTable: "AssessmentModel",
                        principalColumn: "AssessmentId");
                    table.ForeignKey(
                        name: "FK_ResultModel_UserModel",
                        column: x => x.UserId,
                        principalTable: "UserModel",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentModel_CourseId",
                table: "AssessmentModel",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseModel_UserId",
                table: "CourseModel",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultModel_AssessmentId",
                table: "ResultModel",
                column: "AssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultModel_UserId",
                table: "ResultModel",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserModel",
                table: "UserModel",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultModel");

            migrationBuilder.DropTable(
                name: "AssessmentModel");

            migrationBuilder.DropTable(
                name: "CourseModel");

            migrationBuilder.DropTable(
                name: "UserModel");
        }
    }
}
