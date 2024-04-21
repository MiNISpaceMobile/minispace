using Api.DTO.Comments;
using Api.DTO.Events;
using Api.DTO.Posts;
using Api.DTO.Users;
using Domain.DataModel;

namespace Api.DTO;

// Temporarily every Id is just 0 as we don't have Id in data model
public static class MappingExtensions
{
    public static UserDto ToDto(this User user)
    {
        var role = user switch
        {
            Administrator => "ADMIN",
            Student student => student.IsOrganizer ? "ORGANIZER" : null,
            _ => null
        };
        var roles = role is null ? Enumerable.Empty<string>() : [role];
        return new UserDto(default, user.FirstName, user.LastName, default, roles, default);
    }

    public static CommentDto ToDto(this Comment comment)
    {
        return new CommentDto(default, default, comment.Author?.ToDto(), comment.Content);
    }

    // Organizing unit is null as we don't have it in data model
    public static EventDto ToDto(this Event @event)
    {
        return new EventDto(default, @event.Guid, @event.Title, null!, @event.StartDate,
            @event.Description, @event.Participants.Count);
    }

    public static PostDto ToDto(this Post post)
    {
        return new PostDto(default, post.Guid, default, default, post.CreationDate, post.Content);
    }
}