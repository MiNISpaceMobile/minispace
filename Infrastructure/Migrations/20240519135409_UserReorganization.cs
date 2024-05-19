using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserReorganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentStudent");

            migrationBuilder.DropTable(
                name: "EventStudent");

            migrationBuilder.DropTable(
                name: "EventStudent1");

            migrationBuilder.DropTable(
                name: "StudentStudent");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "User");

            migrationBuilder.AlterColumn<bool>(
                name: "IsOrganizer",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "EmailNotification",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "User",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "User",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CommentUser",
                columns: table => new
                {
                    CommentGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    LikersGuid = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentUser", x => new { x.CommentGuid, x.LikersGuid });
                    table.ForeignKey(
                        name: "FK_CommentUser_Comment_CommentGuid",
                        column: x => x.CommentGuid,
                        principalTable: "Comment",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentUser_User_LikersGuid",
                        column: x => x.LikersGuid,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventUser",
                columns: table => new
                {
                    InterestedGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubscribedEventsGuid = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUser", x => new { x.InterestedGuid, x.SubscribedEventsGuid });
                    table.ForeignKey(
                        name: "FK_EventUser_Event_SubscribedEventsGuid",
                        column: x => x.SubscribedEventsGuid,
                        principalTable: "Event",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventUser_User_InterestedGuid",
                        column: x => x.InterestedGuid,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventUser1",
                columns: table => new
                {
                    JoinedEventsGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParticipantsGuid = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUser1", x => new { x.JoinedEventsGuid, x.ParticipantsGuid });
                    table.ForeignKey(
                        name: "FK_EventUser1_Event_JoinedEventsGuid",
                        column: x => x.JoinedEventsGuid,
                        principalTable: "Event",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventUser1_User_ParticipantsGuid",
                        column: x => x.ParticipantsGuid,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserUser",
                columns: table => new
                {
                    FriendsGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserGuid = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUser", x => new { x.FriendsGuid, x.UserGuid });
                    table.ForeignKey(
                        name: "FK_UserUser_User_FriendsGuid",
                        column: x => x.FriendsGuid,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserUser_User_UserGuid",
                        column: x => x.UserGuid,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentUser_LikersGuid",
                table: "CommentUser",
                column: "LikersGuid");

            migrationBuilder.CreateIndex(
                name: "IX_EventUser_SubscribedEventsGuid",
                table: "EventUser",
                column: "SubscribedEventsGuid");

            migrationBuilder.CreateIndex(
                name: "IX_EventUser1_ParticipantsGuid",
                table: "EventUser1",
                column: "ParticipantsGuid");

            migrationBuilder.CreateIndex(
                name: "IX_UserUser_UserGuid",
                table: "UserUser",
                column: "UserGuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentUser");

            migrationBuilder.DropTable(
                name: "EventUser");

            migrationBuilder.DropTable(
                name: "EventUser1");

            migrationBuilder.DropTable(
                name: "UserUser");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "User");

            migrationBuilder.AlterColumn<bool>(
                name: "IsOrganizer",
                table: "User",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "EmailNotification",
                table: "User",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "User",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "User",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "User",
                type: "TEXT",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CommentStudent",
                columns: table => new
                {
                    CommentGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    LikersGuid = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentStudent", x => new { x.CommentGuid, x.LikersGuid });
                    table.ForeignKey(
                        name: "FK_CommentStudent_Comment_CommentGuid",
                        column: x => x.CommentGuid,
                        principalTable: "Comment",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentStudent_User_LikersGuid",
                        column: x => x.LikersGuid,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventStudent",
                columns: table => new
                {
                    InterestedGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubscribedEventsGuid = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStudent", x => new { x.InterestedGuid, x.SubscribedEventsGuid });
                    table.ForeignKey(
                        name: "FK_EventStudent_Event_SubscribedEventsGuid",
                        column: x => x.SubscribedEventsGuid,
                        principalTable: "Event",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventStudent_User_InterestedGuid",
                        column: x => x.InterestedGuid,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventStudent1",
                columns: table => new
                {
                    JoinedEventsGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParticipantsGuid = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStudent1", x => new { x.JoinedEventsGuid, x.ParticipantsGuid });
                    table.ForeignKey(
                        name: "FK_EventStudent1_Event_JoinedEventsGuid",
                        column: x => x.JoinedEventsGuid,
                        principalTable: "Event",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventStudent1_User_ParticipantsGuid",
                        column: x => x.ParticipantsGuid,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentStudent",
                columns: table => new
                {
                    FriendsGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    StudentGuid = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentStudent", x => new { x.FriendsGuid, x.StudentGuid });
                    table.ForeignKey(
                        name: "FK_StudentStudent_User_FriendsGuid",
                        column: x => x.FriendsGuid,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentStudent_User_StudentGuid",
                        column: x => x.StudentGuid,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentStudent_LikersGuid",
                table: "CommentStudent",
                column: "LikersGuid");

            migrationBuilder.CreateIndex(
                name: "IX_EventStudent_SubscribedEventsGuid",
                table: "EventStudent",
                column: "SubscribedEventsGuid");

            migrationBuilder.CreateIndex(
                name: "IX_EventStudent1_ParticipantsGuid",
                table: "EventStudent1",
                column: "ParticipantsGuid");

            migrationBuilder.CreateIndex(
                name: "IX_StudentStudent_StudentGuid",
                table: "StudentStudent",
                column: "StudentGuid");
        }
    }
}
