using Api.DTO.Users;

namespace Api.DTO.Events;

public record EventDto(
    Guid Guid,
    UserDto? Organizer,
    string Title,
    string Description,
    string EventCategory,
    DateTime PublicationDate,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    int ParticipantCount,
    int InterestedCount,
    int ViewCount,
    int? AverageAge,
    IEnumerable<string> PictureUrls);