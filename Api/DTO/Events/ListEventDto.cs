using Api.DTO.Users;

namespace Api.DTO.Events;
public record ListEventDto(
    Guid Guid,
    string Title,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    int ParticipantCount,
    int InterestedCount,
    int? AvailablePlaces,
    decimal? Fee,
    // Image
    float? Rating);