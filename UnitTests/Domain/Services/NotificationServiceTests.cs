using Domain.Abstractions;
using Domain.DataModel;
using Domain.Services.Implementations;
using Infrastructure.EmailSenders;
using Infrastructure.UnitOfWorks;

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
        response = new Comment(organizer, post, "answer to comment", comment);
        uow = new DictionaryUnitOfWork([student, organizer, @event, post, comment, response]);
        notificationService = new(uow, new FakeEmailSender());

    }

    #region GenerateNewEventNotifications
    [TestMethod]
    public void GenerateNewEventNotifications_FriendOrganizesEvent_AddsNotification()
    {
        var notifications = student.SocialNotifications;
        var oldCount = notifications.Count;
        var hasFriendOrganizesEventNotification = notifications.Any(x => x.Type == SocialNotificationType.FriendOrganizesEvent);

        notificationService.GenerateNewEventNotifications(@event);
        var friendOrganizesEventNotification = notifications.FirstOrDefault(x => x.Type == SocialNotificationType.FriendOrganizesEvent);

        Assert.AreNotEqual(notifications.Count, oldCount);
        Assert.AreNotEqual(hasFriendOrganizesEventNotification, friendOrganizesEventNotification is not null);
        Assert.AreEqual(friendOrganizesEventNotification?.SourceId, @event.Guid);
    }
    #endregion GenerateNewEventNotifications

    #region GenerateNewPostNotifications
    [TestMethod]
    public void GenerateNewPostNotifications_SubscribedEventNewPost_AddsNotification()
    {
        var notifications = student.PersonalNotifications;
        var oldCount = notifications.Count;
        var hasEventNewPostNotification = notifications.Any(x => x.Type == NotificationType.EventNewPost);

        notificationService.GenerateNewPostNotifications(post);
        var eventNewPostNotification = notifications.FirstOrDefault(x => x.Type == NotificationType.EventNewPost);

        Assert.AreNotEqual(notifications.Count, oldCount);
        Assert.AreNotEqual(hasEventNewPostNotification, eventNewPostNotification is not null);
        Assert.AreEqual(eventNewPostNotification?.SourceId, post.Guid);
    }
    #endregion GenerateNewPostNotifications

    #region GenerateNewCommentNotifications
    [TestMethod]
    public void GenerateNewCommentNotifications_CommentGetsResponse_AddsNotification()
    {
        var notifications = student.PersonalNotifications;
        var oldCount = notifications.Count;
        var hasCommentRespondedToNotificiation = notifications.Any(x => x.Type == NotificationType.CommentRespondedTo);

        notificationService.GenerateNewCommentNotifications(response);
        var commentRespondedToNotification = notifications.FirstOrDefault(x => x.Type == NotificationType.CommentRespondedTo);

        Assert.AreNotEqual(notifications.Count, oldCount);
        Assert.AreNotEqual(hasCommentRespondedToNotificiation, commentRespondedToNotification is not null);
        Assert.AreEqual(commentRespondedToNotification?.SourceId, response.Guid);
    }

    [TestMethod]
    public void GenerateNewCommentNotifications_FriendCommentedSubscribedEvent_AddsNotification()
    {
        var notifications = student.SocialNotifications;
        var notificationsCount = notifications.Count;
        var hasFriendCommentedNotification = notifications.Any(x => x.Type == SocialNotificationType.FriendCommented);

        notificationService.GenerateNewCommentNotifications(response);
        var friendCommentedNotification = notifications.FirstOrDefault(x => x.Type == SocialNotificationType.FriendCommented);

        Assert.AreNotEqual(notifications.Count, notificationsCount);
        Assert.AreNotEqual(hasFriendCommentedNotification, friendCommentedNotification is not null);
        Assert.AreEqual(friendCommentedNotification?.SourceId, response.Guid);
    }
    #endregion GenerateNewCommentNotifications

    #region GenerateEventStartsSoonNotifications
    [TestMethod]
    public void GenerateEventStartsSoonNotifications()
    {
        var notifications = student.PersonalNotifications;
        var notificationsCount = notifications.Count;
        var hasEventStartsSoonNotification = notifications.Any(x => x.Type == NotificationType.EventStartsSoon);

        notificationService.GenerateEventStartsSoonNotifications();
        var eventsStartsSoonNotification = notifications.FirstOrDefault(x => x.Type == NotificationType.EventStartsSoon);

        Assert.AreNotEqual(notifications.Count, notificationsCount);
        Assert.AreNotEqual(hasEventStartsSoonNotification, eventsStartsSoonNotification is not null);
        Assert.AreEqual(eventsStartsSoonNotification?.SourceId, @event.Guid);
    }
    #endregion GenerateEventStartsSoonNotifications

    #region GenerateJoinedEventNotifications
    [TestMethod]
    public void GenerateJoinedEventNotifications()
    {
        var notifications = organizer.SocialNotifications;
        var notificationsCount = notifications.Count;
        var hasFriendJoinedEventNotification = notifications.Any(x => x.Type == SocialNotificationType.FriendJoinedEvent);

        notificationService.GenerateJoinedEventNotifications(student, @event);
        var friendJoinedEventNotification = notifications.FirstOrDefault(x => x.Type == SocialNotificationType.FriendJoinedEvent);

        Assert.AreNotEqual(notifications.Count, notificationsCount);
        Assert.AreNotEqual(hasFriendJoinedEventNotification, friendJoinedEventNotification is not null);
        Assert.AreEqual(friendJoinedEventNotification?.SourceId, @event.Guid);
    }
    #endregion GenerateJoinedEventNotifications
}
