﻿
using Domain.DataModel;

namespace Domain.Services.Abstractions;

public interface INotificationService
{
    public void GenerateNewEventNotifications(Event @event);
    public void GenerateNewPostNotifications(Post post);
    public void GenerateNewCommentNotifications(Comment comment);
    public void GenerateEventStartsSoonNotifications();
    public void GenerateJoinedEventNotifications(User user, Event @event);

    public void SendEmailIfEnabled(FriendRequest notification);
}