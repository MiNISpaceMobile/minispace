using Api.DTO.Comments;
using Api.DTO.Events;
using Api.DTO.Posts;
using Api.DTO.Reports;
using Api.DTO.Students;
using Api.DTO.Users;
using Domain.DataModel;
using Domain.Services;

namespace Api.DTO;

public static class MappingExtensions
{
    public static UserDto ToDto(this User user) =>
        new(user.Guid, user.FirstName, user.LastName, user.Email);

    public static StudentDto ToDto(this Student Student) =>
        new(Student.Guid, Student.FirstName, Student.LastName, Student.Email,
            Student.Description, Student.DateOfBirth, Student.Age, Student.IsOrganizer);

    public static UserDto ToUserDto(this Student Student) =>
        (Student as User).ToDto();

    public static CommentDto ToDto(this Comment comment) =>
        new(comment.Guid, comment.Author?.ToUserDto(), comment.Content);

    public static PostDto ToDto(this Post post) =>
        new(post.Guid, post.EventId, post.Author?.ToUserDto(), post.CreationDate);

    public static EventDto ToDto(this Event @event) =>
        new(@event.Guid, @event.Organizer?.ToUserDto(), @event.Title, @event.Description,
            @event.Category.ToString(), @event.PublicationDate, @event.StartDate, @event.EndDate,
            @event.Location, @event.Participants.Count, @event.Interested.Count, @event.ViewCount, @event.AverageAge);

    public static ReportDto ToDto(this Report report) =>
        new(report.Guid, report.Author?.ToDto(), report.Responder?.ToDto(), report.TargetId, report.Title,
            report.Details, report.Category.ToString(), report.CreationDate, report.UpdateDate,
            report.Feedback, report.State.ToString(), report.ReportType.ToString());

    public static PagedResponse<R> Map<T, R>(this PagedResponse<T> paged, Func<T, R> mappingFunction) =>
        new()
        {
            Items = paged.Items.Select(mappingFunction),
            PageIndex = paged.PageIndex,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount,
            TotalPages = paged.TotalPages,
            IsLast = paged.IsLast
        };
}