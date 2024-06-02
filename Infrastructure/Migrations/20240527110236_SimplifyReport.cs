using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Report");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Report",
                newName: "ReportType");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Report",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsOpen",
                table: "Report",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Report",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "IsOpen",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Report");

            migrationBuilder.RenameColumn(
                name: "ReportType",
                table: "Report",
                newName: "State");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Report",
                type: "TEXT",
                unicode: false,
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }
    }
}
