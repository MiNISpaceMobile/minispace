using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services;
using Domain.Services.Implementations;
using Infrastructure.Storages;
using Infrastructure.UnitOfWorks;
using Microsoft.Extensions.Logging;

namespace UnitTests.Domain.Services;

[TestClass]
public class NotificationServiceTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private IUnitOfWork uow;
    private User student;
    private User organizer;
    private Event @event;
    private Post post;
    private Comment comment;
    private Comment response;
    private NotificationService notificationService;
#pragma warning restore CS8618 // Unassigned non-nullables
    [TestInitialize]
    public void Setup()
    {
        var now = DateTime.Now;
        student = new User("user0", "user0@test.pl", "password", now);
        organizer = new User("user1", "user1@test.pl", "password", now) { IsOrganizer = true };
        student.Friends.Add(organizer);
        organizer.Friends.Add(student);

        @event = new Event(organizer, "test event", "test description", EventCategory.Uncategorized,
            now, now.AddDays(2), now.AddDays(2), "test location", 20, 20);
        @event.Participants.Add(student);
        student.JoinedEvents.Add(@event);

        post = new Post(organizer, @event, "title", "post");
        comment = new Comment(student, post, "first comment", null);
        response = new Comment(organizer, post, "answer to commen", comment);
        uow = new DictionaryUnitOfWork([student, organizer, @event, post, comment]);
        notificationService = new(uow);

    }

    #region GenerateNewEventNotifications
    [TestMethod]
    public void GenerateNewEventNotifications_FriendOrganizesEvent_AddsNotification()
    {
        var notificationsCount = student.SocialNotifications.Count;
        var hasNewEventNotification = student.SocialNotifications.Any(x => x.Type == SocialNotificationType.FriendOrganizesEvent);

        notificationService.GenerateNewEventNotifications(@event);
        var notifications = student.SocialNotifications;
        var eventNotification = notifications.FirstOrDefault(x => x.Type == SocialNotificationType.FriendOrganizesEvent);

        Assert.AreNotEqual(notifications.Count, notificationsCount);
        Assert.IsNotNull(eventNotification);
        Assert.AreNotEqual(hasNewEventNotification, eventNotification is not null);
        Assert.AreEqual(eventNotification?.SourceId, @event.Guid);
    }
    #endregion GenerateNewEventNotifications

    #region GenerateNewPostNotifications
    [TestMethod]
    public void GenerateNewPostNotifications_SubscribedEventNewPost_AddsNotification()
    {
        var notificationsCount = student.PersonalNotifications.Count;
        var hasNewPostNotification = student.PersonalNotifications.Any(x => x.Type == NotificationType.EventNewPost);

        notificationService.GenerateNewPostNotifications(post);
        var notifications = student.PersonalNotifications;
        var postNotification = notifications.FirstOrDefault(x => x.Type == NotificationType.EventNewPost);

        Assert.AreNotEqual(notifications.Count, notificationsCount);
        Assert.IsNotNull(postNotification);
        Assert.AreNotEqual(hasNewPostNotification, postNotification is not null);
        Assert.AreEqual(postNotification?.SourceId, post.Guid);
    }
    #endregion GenerateNewPostNotifications

    #region GenerateNewCommentNotifications
    [TestMethod]
    public void GenerateNewCommentNotifications_CommentGetsResponse_AddsNotification()
    {
        var notificationsCount = student.PersonalNotifications.Count;
        var hasNewResponseNotification = student.PersonalNotifications.Any(x => x.Type == NotificationType.CommentReponsedTo);

        notificationService.GenerateNewCommentNotifications(response);
        var notifications = student.PersonalNotifications;
        var responseNotification = notifications.FirstOrDefault(x => x.Type == NotificationType.CommentReponsedTo);

        Assert.AreNotEqual(notifications.Count, notificationsCount);
        Assert.IsNotNull(responseNotification);
        Assert.AreNotEqual(hasNewResponseNotification, responseNotification is not null);
        Assert.AreEqual(responseNotification?.SourceId, response.Guid);
    }

    [TestMethod]
    public void GenerateNewCommentNotifications_FriendCommentedSubscribedEvent_AddsNotification()
    {
        var notificationsCount = student.SocialNotifications.Count;
        var hasFriendCommentedNotification = student.SocialNotifications.Any(x => x.Type == SocialNotificationType.FriendCommented);

        notificationService.GenerateNewCommentNotifications(response);
        var notifications = student.SocialNotifications;
        var friendCommentedNotification = notifications.FirstOrDefault(x => x.Type == SocialNotificationType.FriendCommented);

        Assert.AreNotEqual(notifications.Count, notificationsCount);
        Assert.IsNotNull(friendCommentedNotification);
        Assert.AreNotEqual(hasFriendCommentedNotification, friendCommentedNotification is not null);
        Assert.AreEqual(friendCommentedNotification?.SourceId, response.Guid);
    }
    #endregion GenerateNewCommentNotifications

    #region GenerateEventStartsSoonNotifications
    [TestMethod]
    public void GenerateEventStartsSoonNotifications()
    {
        var notificationsCount = student.PersonalNotifications.Count;
        var hasEventStartsSoonNotification = student.PersonalNotifications.Any(x => x.Type == NotificationType.EventStartsSoon);

        notificationService.GenerateEventStartsSoonNotifications();
        var notifications = student.PersonalNotifications;
        var eventsStartsSoonNotification = notifications.FirstOrDefault(x => x.Type == NotificationType.EventStartsSoon);

        Assert.AreNotEqual(notifications.Count, notificationsCount);
        Assert.IsNotNull(eventsStartsSoonNotification);
        Assert.AreNotEqual(hasEventStartsSoonNotification, eventsStartsSoonNotification is not null);
        Assert.AreEqual(eventsStartsSoonNotification?.SourceId, @event.Guid);
    }
    #endregion GenerateEventStartsSoonNotifications

    #region GenerateJoinedEventNotificatons
    [TestMethod]
    public void GenerateJoinedEventNotificatons()
    {
        var notificationsCount = organizer.SocialNotifications.Count;
        var hasFriendJoinedEventNotification = organizer.SocialNotifications.Any(x => x.Type == SocialNotificationType.FriendJoinedEvent);

        notificationService.GenerateJoinedEventNotificatons(student, @event);
        var notifications = organizer.SocialNotifications;
        var FriendJoinedEventNotification = notifications.FirstOrDefault(x => x.Type == SocialNotificationType.FriendJoinedEvent);

        Assert.AreNotEqual(notifications.Count, notificationsCount);
        Assert.IsNotNull(FriendJoinedEventNotification);
        Assert.AreNotEqual(hasFriendJoinedEventNotification, FriendJoinedEventNotification is not null);
        Assert.AreEqual(FriendJoinedEventNotification?.SourceId, @event.Guid);
    }
    #endregion GenerateJoinedEventNotificatons
}
