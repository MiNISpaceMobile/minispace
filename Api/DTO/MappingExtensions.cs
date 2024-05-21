using Api.DTO.Comments;
using Api.DTO.Events;
using Api.DTO.Notifications;
using Api.DTO.Posts;
using Api.DTO.Users;
using Domain.DataModel;

namespace Api.DTO;

public static class MappingExtensions
{
    public static UserDto ToDto(this User user) =>
        new(user.Guid, user.FirstName, user.LastName, user.Email, user.Description,
            user.DateOfBirth, user.IsAdmin, user.IsOrganizer, user.EmailNotification);

    public static CommentDto ToDto(this Comment comment) =>
        new(comment.Guid, comment.Author?.ToDto(), comment.Content);

    public static PostDto ToDto(this Post post) =>
        new(post.Guid, post.EventId, post.Author?.ToDto(), post.CreationDate);

    public static EventDto ToDto(this Event @event) =>
        new(@event.Guid, @event.Organizer?.ToDto(), @event.Title, @event.Description,
            @event.Category.ToString(), @event.PublicationDate, @event.StartDate, @event.EndDate,
            @event.Location, @event.Participants.Count, @event.Interested.Count, @event.ViewCount, @event.AverageAge);

    public static NotificationDto ToDto(this BaseNotification notification) =>
        new(notification.Guid, notification.SourceId, notification.TypeString,
            notification.Seen, notification.Timestamp);
}