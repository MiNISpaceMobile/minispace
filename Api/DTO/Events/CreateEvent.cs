using Domain.DataModel;

namespace Api.DTO.Events;

public record CreateEvent(
    string Title,
    string Description,
    EventCategory EventCategory,
    DateTime PublicationDate,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    int? Capacity,
    int? Fee);
