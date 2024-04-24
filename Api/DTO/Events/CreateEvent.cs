namespace Api.DTO.Events;

public record CreateEvent(
    Guid OrganizerGuid,
    string Title,
    string Description,
    string EventCategory,
    DateTime PublicationDate,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    int? Capacity,
    int? Fee);