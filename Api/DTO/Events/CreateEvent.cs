namespace Api.DTO.Events;

public record CreateEvent(
    string Title,
    string Description,
    string EventCategory,
    DateTime PublicationDate,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    int? Capacity,
    int? Fee);