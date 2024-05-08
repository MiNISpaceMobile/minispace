using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventStudent1_Event_EventGuid",
                table: "EventStudent1");

            migrationBuilder.RenameColumn(
                name: "EventGuid",
                table: "EventStudent1",
                newName: "JoinedEventsGuid");

            migrationBuilder.CreateTable(
                name: "BaseNotification",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Seen = table.Column<bool>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    AuthorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", unicode: false, maxLength: 32, nullable: true),
                    FriendId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SocialNotification_Type = table.Column<string>(type: "TEXT", unicode: false, maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseNotification", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_BaseNotification_User_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaseNotification_User_FriendId",
                        column: x => x.FriendId,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaseNotification_User_TargetId",
                        column: x => x.TargetId,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseNotification_AuthorId",
                table: "BaseNotification",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseNotification_FriendId",
                table: "BaseNotification",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseNotification_TargetId",
                table: "BaseNotification",
                column: "TargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventStudent1_Event_JoinedEventsGuid",
                table: "EventStudent1",
                column: "JoinedEventsGuid",
                principalTable: "Event",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventStudent1_Event_JoinedEventsGuid",
                table: "EventStudent1");

            migrationBuilder.DropTable(
                name: "BaseNotification");

            migrationBuilder.RenameColumn(
                name: "JoinedEventsGuid",
                table: "EventStudent1",
                newName: "EventGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_EventStudent1_Event_EventGuid",
                table: "EventStudent1",
                column: "EventGuid",
                principalTable: "Event",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
