using Api.DTO.Comments;
using Api.DTO.Events;
using Api.DTO.Notifications;
using Api.DTO.Posts;
using Api.DTO.Users;
using Domain.DataModel;

namespace Api.DTO;

public static class MappingExtensions
{
    public static PublicUserDto ToDto(this User user) =>
        new(user.Guid, user.FirstName, user.LastName, user.Description);

    public static PrivateUserDto ToPrivateDto(this User user) =>
        new(user.Guid, user.FirstName, user.LastName, user.Email, user.Description,
            user.DateOfBirth, user.IsAdmin, user.IsOrganizer, user.EmailNotification, user.ProfilePictureUrl);

    public static CommentDto ToDto(this Comment comment) =>
        new(comment.Guid, comment.Author?.ToDto(), comment.Content);

    public static PostDto ToDto(this Post post) =>
        new(post.Guid, post.EventId, post.Author?.ToDto(), post.CreationDate,
            post.Pictures.OrderBy(x => x.Index).Select(x => x.Url));

    public static EventDto ToDto(this Event @event) =>
        new(@event.Guid, @event.Organizer?.ToDto(), @event.Title, @event.Description,
            @event.Category.ToString(), @event.PublicationDate, @event.StartDate, @event.EndDate,
            @event.Location, @event.Participants.Count, @event.Interested.Count, @event.ViewCount, @event.AverageAge,
            @event.Pictures.OrderBy(x => x.Index).Select(x => x.Url));

    public static NotificationDto ToDto(this BaseNotification notification) =>
        new(notification.Guid, notification.SourceId, notification.TypeString,
            notification.Seen, notification.Timestamp);

    public static FriendRequestDto ToDto(this FriendRequest friendRequest) =>
        new(friendRequest.Guid, friendRequest.TargetId, friendRequest.Timestamp, friendRequest.Author.ToDto());
}
