using Domain.Abstractions;
using Domain.DataModel;
using Domain.Services.Abstractions;
using System.Text;

namespace Domain.Services.Implementations;

public class NotificationService(IUnitOfWork uow, IEmailSender sender) : INotificationService
{
    public void GenerateNewEventNotifications(Event @event)
    {
        foreach (var friend in @event.Organizer!.Friends)
        {
            var not = new SocialNotification(friend, @event.Organizer, @event, SocialNotificationType.FriendOrganizesEvent);
            friend.SocialNotifications.Add(not);
            SendEmailIfEnabled(not);
        }
        uow.Commit();
    }

    public void GenerateNewPostNotifications(Post post)
    {
        foreach (var user in post.Event.Interested.Concat(post.Event.Participants))
        {
            var not = new Notification(user, post, NotificationType.EventNewPost);
            user.PersonalNotifications.Add(not);
            SendEmailIfEnabled(not);
        }
        uow.Commit();
    }

    public void GenerateNewCommentNotifications(Comment comment)
    {
        comment.InResponseTo?.Author?.PersonalNotifications.Add(new Notification(comment.InResponseTo.Author, comment, NotificationType.CommentRespondedTo));
        var @event = comment.Post.Event;
        foreach (var friend in comment.Author!.Friends)
        {
            if (@event.Interested.Contains(friend) || @event.Participants.Contains(friend))
            {
                var not = new SocialNotification(friend, comment.Author, comment, SocialNotificationType.FriendCommented);
                friend.SocialNotifications.Add(not);
                SendEmailIfEnabled(not);
            }
        }
        uow.Commit();
    }

    public void GenerateEventStartsSoonNotifications()
    {
        foreach (var @event in uow.Repository<Event>().GetAll())
        {
            if (@event.StartDate > DateTime.Now && @event.StartDate - DateTime.Now < TimeSpan.FromDays(3))
            {
                foreach (var user in @event.Participants.Concat(@event.Interested))
                {
                    var not = new Notification(user, @event, NotificationType.EventStartsSoon);
                    user.PersonalNotifications.Add(not);
                    SendEmailIfEnabled(not);
                }
            }
        }
        uow.Commit();
    }

    public void GenerateJoinedEventNotifications(User user, Event @event)
    {
        foreach (var friend in user.Friends)
        {
            var not = new SocialNotification(friend, user, @event, SocialNotificationType.FriendJoinedEvent);
            friend.SocialNotifications.Add(not);
            SendEmailIfEnabled(not);
        }
        uow.Commit();
    }

    private void SendEmailIfEnabled(Notification notification)
    {
        User u = notification.Target;
        if (!u.EmailNotification || string.IsNullOrEmpty(u.Email))
            return;

        var sb = new StringBuilder();
        sb.Append($"Hi {u.FirstName}!\n\n");
        switch (notification.Type)
        {
            case NotificationType.EventNewPost:
                sb.Append("There is a new post you might be interested in!");
                break;
            case NotificationType.EventStartsSoon:
                sb.Append("One of the events you want to participate in is starting soon!");
                break;
            case NotificationType.CommentRespondedTo:
                sb.Append("Someone responded to your comment!");
                break;
            default:
                return;
        }
        sb.Append($"\n\nBest regards,\nMiniSpace Team\n");
        var content = sb.Replace("\n", "<br>").ToString();

        sender.SendEmail(u.Email, "New notification", content);
    }

    private void SendEmailIfEnabled(SocialNotification notification)
    {
        User u = notification.Target;
        if (!u.EmailNotification || string.IsNullOrEmpty(u.Email))
            return;

        var sb = new StringBuilder();
        sb.Append($"Hi {u.FirstName}!\n\n");
        switch (notification.Type)
        {
            case SocialNotificationType.FriendCommented:
                sb.Append($"Your friend {notification.Friend.FirstName} just posted a comment!");
                break;
            case SocialNotificationType.FriendJoinedEvent:
                sb.Append($"Your friend {notification.Friend.FirstName} just joined an event!");
                break;
            case SocialNotificationType.FriendOrganizesEvent:
                sb.Append($"Your friend {notification.Friend.FirstName} organizes a new event!");
                break;
            default:
                return;
        }
        sb.Append($"\n\nBest regards,\nMiniSpace Team\n");
        var content = sb.Replace("\n", "<br>").ToString();

        sender.SendEmail(u.Email, "New notification from you friend", content);
    }

    public void SendEmailIfEnabled(FriendRequest notification)
    {
        User u = notification.Target;
        if (!u.EmailNotification || string.IsNullOrEmpty(u.Email))
            return;
        User a = notification.Author;

        var sb = new StringBuilder();
        sb.Append($"Hi {u.FirstName}!\n\n");
        sb.Append($"{a.FirstName} {a.LastName} wants to become your friend!");
        sb.Append($"\n\nBest regards,\nMiniSpace Team\n");
        var content = sb.Replace("\n", "<br>").ToString();

        sender.SendEmail(u.Email, "New notification from you friend", content);
    }
}
