using Api.DTO.Comments;
using Api.DTO.Events;
using Api.DTO.Posts;
using Api.DTO.Students;
using Api.DTO.Users;
using Domain.DataModel;

namespace Api.DTO;

public static class MappingExtensions
{
    public static UserDto ToDto(this User user) =>
        new(user.Guid, user.FirstName, user.LastName, user.Email, user.HasProfilePicture);

    public static StudentDto ToDto(this Student Student) =>
        new(Student.Guid, Student.FirstName, Student.LastName, Student.Email,
            Student.Description, Student.DateOfBirth, Student.Age, Student.IsOrganizer);

    public static UserDto ToUserDto(this Student Student) =>
        (Student as User).ToDto();

    public static CommentDto ToDto(this Comment comment) =>
        new(comment.Guid, comment.Author?.ToUserDto(), comment.Content);

    public static PostDto ToDto(this Post post) =>
        new(post.Guid, post.EventId, post.Author?.ToUserDto(), post.CreationDate, post.PictureCount);

    public static EventDto ToDto(this Event @event) =>
        new(@event.Guid, @event.Organizer?.ToUserDto(), @event.Title, @event.Description,
            @event.Category.ToString(), @event.PublicationDate, @event.StartDate, @event.EndDate,
            @event.Location, @event.Participants.Count, @event.Interested.Count, @event.ViewCount,
            @event.AverageAge, @event.PictureCount);
}