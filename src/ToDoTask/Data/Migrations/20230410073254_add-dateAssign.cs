using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoTask.Data.Migrations
{
    /// <inheritdoc />
    public partial class adddateAssign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateAssign",
                table: "UserJobs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAssign",
                table: "UserJobs");
        }
    }
}
