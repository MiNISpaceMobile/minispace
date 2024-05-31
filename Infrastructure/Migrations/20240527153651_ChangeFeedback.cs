using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Feedback");

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Feedback",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Feedback");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Feedback",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
