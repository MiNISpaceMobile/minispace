using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "User",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_ExternalId",
                table: "User",
                column: "ExternalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_ExternalId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "User");
        }
    }
}
