using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoTask.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropUserJobTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserJobs");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateLine",
                table: "ProjectVm",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateLine",
                table: "ProjectUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateLine",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAssign",
                table: "Jobs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateLine",
                table: "ProjectVm");

            migrationBuilder.DropColumn(
                name: "DateLine",
                table: "ProjectUsers");

            migrationBuilder.DropColumn(
                name: "DateLine",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DateAssign",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Jobs");

            migrationBuilder.CreateTable(
                name: "UserJobs",
                columns: table => new
                {
                    JobId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    DateAssign = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJobs", x => new { x.JobId, x.UserId });
                });
        }
    }
}
