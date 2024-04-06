using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ProfilePicture = table.Column<string>(type: "TEXT", nullable: true),
                    SaltedPasswordHash = table.Column<byte[]>(type: "BLOB", fixedLength: true, maxLength: 64, nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmailNotification = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsOrganizer = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", unicode: false, maxLength: 32, nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: true),
                    Fee = table.Column<decimal>(type: "TEXT", nullable: true),
                    Feedback = table.Column<string>(type: "TEXT", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Event_User_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.SetNull);
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
                    EventGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParticipantsGuid = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStudent1", x => new { x.EventGuid, x.ParticipantsGuid });
                    table.ForeignKey(
                        name: "FK_EventStudent1_Event_EventGuid",
                        column: x => x.EventGuid,
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
                name: "Post",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Post_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_User_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PostId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InResponeseToId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Comment_Comment_InResponeseToId",
                        column: x => x.InResponeseToId,
                        principalTable: "Comment",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comment_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comment_User_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.SetNull);
                });

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
                name: "Report",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ResponderId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Details = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", unicode: false, maxLength: 32, nullable: false),
                    Feedback = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", unicode: false, maxLength: 32, nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    ReportedCommentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ReportedEventId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ReportedPostId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Report_Comment_ReportedCommentId",
                        column: x => x.ReportedCommentId,
                        principalTable: "Comment",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Report_Event_ReportedEventId",
                        column: x => x.ReportedEventId,
                        principalTable: "Event",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Report_Post_ReportedPostId",
                        column: x => x.ReportedPostId,
                        principalTable: "Post",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Report_User_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Report_User_ResponderId",
                        column: x => x.ResponderId,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AuthorId",
                table: "Comment",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_InResponeseToId",
                table: "Comment",
                column: "InResponeseToId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId",
                table: "Comment",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentStudent_LikersGuid",
                table: "CommentStudent",
                column: "LikersGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Event_OrganizerId",
                table: "Event",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_EventStudent_SubscribedEventsGuid",
                table: "EventStudent",
                column: "SubscribedEventsGuid");

            migrationBuilder.CreateIndex(
                name: "IX_EventStudent1_ParticipantsGuid",
                table: "EventStudent1",
                column: "ParticipantsGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Post_AuthorId",
                table: "Post",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_EventId",
                table: "Post",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_AuthorId",
                table: "Report",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportedCommentId",
                table: "Report",
                column: "ReportedCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportedEventId",
                table: "Report",
                column: "ReportedEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportedPostId",
                table: "Report",
                column: "ReportedPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ResponderId",
                table: "Report",
                column: "ResponderId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentStudent_StudentGuid",
                table: "StudentStudent",
                column: "StudentGuid");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentStudent");

            migrationBuilder.DropTable(
                name: "EventStudent");

            migrationBuilder.DropTable(
                name: "EventStudent1");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "StudentStudent");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
