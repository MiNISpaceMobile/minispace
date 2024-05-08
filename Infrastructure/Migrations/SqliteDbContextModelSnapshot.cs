﻿// <auto-generated />
using System;
using Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(SqliteDbContext))]
    partial class SqliteDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("CommentStudent", b =>
                {
                    b.Property<Guid>("CommentGuid")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("LikersGuid")
                        .HasColumnType("TEXT");

                    b.HasKey("CommentGuid", "LikersGuid");

                    b.HasIndex("LikersGuid");

                    b.ToTable("CommentStudent");
                });

            modelBuilder.Entity("Domain.DataModel.BaseNotification", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("TEXT");

                    b.Property<bool>("Seen")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("SourceId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TargetId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Guid");

                    b.ToTable("BaseNotification");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BaseNotification");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Domain.DataModel.Comment", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("InResponeseToId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("PostId")
                        .HasColumnType("TEXT");

                    b.HasKey("Guid");

                    b.HasIndex("AuthorId");

                    b.HasIndex("InResponeseToId");

                    b.HasIndex("PostId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("Domain.DataModel.Event", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int?>("Capacity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(32)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("Fee")
                        .HasColumnType("TEXT");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("OrganizerId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PublicationDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ViewCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("Guid");

                    b.HasIndex("OrganizerId");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("Domain.DataModel.Feedback", b =>
                {
                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("EventId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AuthorId", "EventId");

                    b.HasIndex("EventId");

                    b.ToTable("Feedback");
                });

            modelBuilder.Entity("Domain.DataModel.Post", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("EventId")
                        .HasColumnType("TEXT");

                    b.HasKey("Guid");

                    b.HasIndex("AuthorId");

                    b.HasIndex("EventId");

                    b.ToTable("Post");
                });

            modelBuilder.Entity("Domain.DataModel.Report", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(32)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("TEXT");

                    b.Property<string>("Feedback")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ResponderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasMaxLength(32)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Guid");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ResponderId");

                    b.ToTable("Report");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Report");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Domain.DataModel.User", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("ExternalId")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.HasKey("Guid");

                    b.HasIndex("ExternalId")
                        .IsUnique();

                    b.ToTable("User");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("EventStudent", b =>
                {
                    b.Property<Guid>("InterestedGuid")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SubscribedEventsGuid")
                        .HasColumnType("TEXT");

                    b.HasKey("InterestedGuid", "SubscribedEventsGuid");

                    b.HasIndex("SubscribedEventsGuid");

                    b.ToTable("EventStudent");
                });

            modelBuilder.Entity("EventStudent1", b =>
                {
                    b.Property<Guid>("JoinedEventsGuid")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ParticipantsGuid")
                        .HasColumnType("TEXT");

                    b.HasKey("JoinedEventsGuid", "ParticipantsGuid");

                    b.HasIndex("ParticipantsGuid");

                    b.ToTable("EventStudent1");
                });

            modelBuilder.Entity("StudentStudent", b =>
                {
                    b.Property<Guid>("FriendsGuid")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("StudentGuid")
                        .HasColumnType("TEXT");

                    b.HasKey("FriendsGuid", "StudentGuid");

                    b.HasIndex("StudentGuid");

                    b.ToTable("StudentStudent");
                });

            modelBuilder.Entity("Domain.DataModel.FriendRequest", b =>
                {
                    b.HasBaseType("Domain.DataModel.BaseNotification");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.HasIndex("AuthorId");

                    b.HasIndex("TargetId");

                    b.HasDiscriminator().HasValue("FriendRequest");
                });

            modelBuilder.Entity("Domain.DataModel.Notification", b =>
                {
                    b.HasBaseType("Domain.DataModel.BaseNotification");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(32)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.HasIndex("TargetId");

                    b.HasDiscriminator().HasValue("Notification");
                });

            modelBuilder.Entity("Domain.DataModel.SocialNotification", b =>
                {
                    b.HasBaseType("Domain.DataModel.BaseNotification");

                    b.Property<Guid>("FriendId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(32)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.HasIndex("FriendId");

                    b.HasIndex("TargetId");

                    b.ToTable("BaseNotification", t =>
                        {
                            t.Property("Type")
                                .HasColumnName("SocialNotification_Type");
                        });

                    b.HasDiscriminator().HasValue("SocialNotification");
                });

            modelBuilder.Entity("Domain.DataModel.CommentReport", b =>
                {
                    b.HasBaseType("Domain.DataModel.Report");

                    b.Property<Guid>("ReportedCommentId")
                        .HasColumnType("TEXT");

                    b.HasIndex("ReportedCommentId");

                    b.HasDiscriminator().HasValue("CommentReport");
                });

            modelBuilder.Entity("Domain.DataModel.EventReport", b =>
                {
                    b.HasBaseType("Domain.DataModel.Report");

                    b.Property<Guid>("ReportedEventId")
                        .HasColumnType("TEXT");

                    b.HasIndex("ReportedEventId");

                    b.HasDiscriminator().HasValue("EventReport");
                });

            modelBuilder.Entity("Domain.DataModel.PostReport", b =>
                {
                    b.HasBaseType("Domain.DataModel.Report");

                    b.Property<Guid>("ReportedPostId")
                        .HasColumnType("TEXT");

                    b.HasIndex("ReportedPostId");

                    b.HasDiscriminator().HasValue("PostReport");
                });

            modelBuilder.Entity("Domain.DataModel.Administrator", b =>
                {
                    b.HasBaseType("Domain.DataModel.User");

                    b.HasDiscriminator().HasValue("Administrator");
                });

            modelBuilder.Entity("Domain.DataModel.Student", b =>
                {
                    b.HasBaseType("Domain.DataModel.User");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailNotification")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsOrganizer")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("Student");
                });

            modelBuilder.Entity("CommentStudent", b =>
                {
                    b.HasOne("Domain.DataModel.Comment", null)
                        .WithMany()
                        .HasForeignKey("CommentGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.DataModel.Student", null)
                        .WithMany()
                        .HasForeignKey("LikersGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.DataModel.Comment", b =>
                {
                    b.HasOne("Domain.DataModel.Student", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Domain.DataModel.Comment", "InResponseTo")
                        .WithMany("Responses")
                        .HasForeignKey("InResponeseToId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Domain.DataModel.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("InResponseTo");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("Domain.DataModel.Event", b =>
                {
                    b.HasOne("Domain.DataModel.Student", "Organizer")
                        .WithMany("OrganizedEvents")
                        .HasForeignKey("OrganizerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Organizer");
                });

            modelBuilder.Entity("Domain.DataModel.Feedback", b =>
                {
                    b.HasOne("Domain.DataModel.Student", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.DataModel.Event", "Event")
                        .WithMany("Feedback")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("Domain.DataModel.Post", b =>
                {
                    b.HasOne("Domain.DataModel.Student", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Domain.DataModel.Event", "Event")
                        .WithMany("Posts")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("Domain.DataModel.Report", b =>
                {
                    b.HasOne("Domain.DataModel.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Domain.DataModel.Administrator", "Responder")
                        .WithMany()
                        .HasForeignKey("ResponderId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Author");

                    b.Navigation("Responder");
                });

            modelBuilder.Entity("EventStudent", b =>
                {
                    b.HasOne("Domain.DataModel.Student", null)
                        .WithMany()
                        .HasForeignKey("InterestedGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.DataModel.Event", null)
                        .WithMany()
                        .HasForeignKey("SubscribedEventsGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EventStudent1", b =>
                {
                    b.HasOne("Domain.DataModel.Event", null)
                        .WithMany()
                        .HasForeignKey("JoinedEventsGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.DataModel.Student", null)
                        .WithMany()
                        .HasForeignKey("ParticipantsGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("StudentStudent", b =>
                {
                    b.HasOne("Domain.DataModel.Student", null)
                        .WithMany()
                        .HasForeignKey("FriendsGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.DataModel.Student", null)
                        .WithMany()
                        .HasForeignKey("StudentGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.DataModel.FriendRequest", b =>
                {
                    b.HasOne("Domain.DataModel.Student", "Author")
                        .WithMany("SentFriendRequests")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.DataModel.Student", "Target")
                        .WithMany("ReceivedFriendRequests")
                        .HasForeignKey("TargetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Target");
                });

            modelBuilder.Entity("Domain.DataModel.Notification", b =>
                {
                    b.HasOne("Domain.DataModel.Student", "Target")
                        .WithMany("PersonalNotifications")
                        .HasForeignKey("TargetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Target");
                });

            modelBuilder.Entity("Domain.DataModel.SocialNotification", b =>
                {
                    b.HasOne("Domain.DataModel.Student", "Friend")
                        .WithMany()
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.DataModel.Student", "Target")
                        .WithMany("SocialNotifications")
                        .HasForeignKey("TargetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Friend");

                    b.Navigation("Target");
                });

            modelBuilder.Entity("Domain.DataModel.CommentReport", b =>
                {
                    b.HasOne("Domain.DataModel.Comment", "ReportedComment")
                        .WithMany()
                        .HasForeignKey("ReportedCommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReportedComment");
                });

            modelBuilder.Entity("Domain.DataModel.EventReport", b =>
                {
                    b.HasOne("Domain.DataModel.Event", "ReportedEvent")
                        .WithMany()
                        .HasForeignKey("ReportedEventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReportedEvent");
                });

            modelBuilder.Entity("Domain.DataModel.PostReport", b =>
                {
                    b.HasOne("Domain.DataModel.Post", "ReportedPost")
                        .WithMany()
                        .HasForeignKey("ReportedPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReportedPost");
                });

            modelBuilder.Entity("Domain.DataModel.Comment", b =>
                {
                    b.Navigation("Responses");
                });

            modelBuilder.Entity("Domain.DataModel.Event", b =>
                {
                    b.Navigation("Feedback");

                    b.Navigation("Posts");
                });

            modelBuilder.Entity("Domain.DataModel.Post", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("Domain.DataModel.Student", b =>
                {
                    b.Navigation("OrganizedEvents");

                    b.Navigation("PersonalNotifications");

                    b.Navigation("ReceivedFriendRequests");

                    b.Navigation("SentFriendRequests");

                    b.Navigation("SocialNotifications");
                });
#pragma warning restore 612, 618
        }
    }
}
