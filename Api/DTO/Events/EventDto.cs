using Api.DTO.Users;

namespace Api.DTO.Events;

public record EventDto(
    Guid Guid,
    PublicUserDto? Organizer,
    string Title,
    string Description,
    string EventCategory,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    int ParticipantCount,
    int InterestedCount,
    int ViewCount,
    decimal? Fee,
    int? Capacity,
    int? Availableplaces,
    int? AverageAge,
    float? Rating,
    IEnumerable<string> PictureUrls);