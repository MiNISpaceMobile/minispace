
using Domain.DataModel;

namespace Domain.Services.Abstractions;

public interface INotificationService
{
    public void GenerateNewEventNotifications(Event @event);
    public void GenerateNewPostNotifications(Post post);
    public void GenerateNewCommentNotifications(Comment comment);
    public void GenerateEventStartsSoonNotifications();
    public void GenerateJoinedEventNotificatons(User user, Event @event);
}