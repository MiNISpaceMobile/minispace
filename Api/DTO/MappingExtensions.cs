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

    public static EventDto ToDto(this Event @event)
    {
        int? avPlaces = null;
        if (@event.Capacity is not null)
            avPlaces = @event.Capacity - @event.Participants.Count;
        return new(@event.Guid, @event.Organizer?.ToDto(), @event.Title, @event.Description,
            @event.Category.ToString(), @event.StartDate, @event.EndDate,
            @event.Location, @event.Participants.Count, @event.Interested.Count, @event.ViewCount, @event.Fee, @event.Capacity, avPlaces, @event.AverageAge);
    }

    public static ListEventDto ToListEventDto(this Event e)
    {
        int? avPlaces = null;
        if (e.Capacity is not null)
            avPlaces = e.Capacity - e.Participants.Count;
        return new ListEventDto(e.Guid, e.Title, e.StartDate, e.EndDate, e.Location, e.Participants.Count, e.Interested.Count, avPlaces, e.Fee, null);
    }

    public static NotificationDto ToDto(this BaseNotification notification) =>
        new(notification.Guid, notification.SourceId, notification.TypeString,
            notification.Seen, notification.Timestamp);

    public static FriendRequestDto ToDto(this FriendRequest friendRequest) =>
        new(friendRequest.Guid, friendRequest.TargetId, friendRequest.AuthorId, friendRequest.Timestamp);
}