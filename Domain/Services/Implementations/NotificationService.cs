using Domain.Abstractions;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class NotificationService(IUnitOfWork uow) : INotificationService
{
    public void GenerateNewEventNotifications(Event @event)
    {
        foreach (var friend in @event.Organizer!.Friends)
            friend.SocialNotifications.Add(new SocialNotification(friend, @event.Organizer, @event, SocialNotificationType.FriendOrganizesEvent));
        uow.Commit();
    }

    public void GenerateNewPostNotifications(Post post)
    {
        foreach (var user in post.Event.Interested.Concat(post.Event.Participants))
            user.PersonalNotifications.Add(new Notification(user, post, NotificationType.EventNewPost));
        uow.Commit();
    }

    public void GenerateNewCommentNotifications(Comment comment)
    {
        comment.InResponseTo?.Author?.PersonalNotifications.Add(new Notification(comment.InResponseTo.Author, comment, NotificationType.CommentReponsedTo));
        var @event = comment.Post.Event;
        foreach (var friend in comment.Author!.Friends)
            if (@event.Interested.Contains(friend) || @event.Participants.Contains(friend))
                friend.SocialNotifications.Add(new SocialNotification(friend, comment.Author, comment, SocialNotificationType.FriendCommented));
        uow.Commit();
    }

    public void GenerateEventStartsSoonNotifications()
    {
        foreach (var @event in uow.Repository<Event>().GetAll())
            if (@event.StartDate > DateTime.Now && @event.StartDate - DateTime.Now < TimeSpan.FromDays(3))
                foreach (var user in @event.Participants.Concat(@event.Interested))
                    user.PersonalNotifications.Add(new Notification(user, @event, NotificationType.EventStartsSoon));
        uow.Commit();
    }

    public void GenerateJoinedEventNotifications(User user, Event @event)
    {
        foreach (var friend in user.Friends)
            friend.SocialNotifications.Add(new SocialNotification(friend, user, @event, SocialNotificationType.FriendJoinedEvent));
        uow.Commit();
    }
}
