
namespace Api.DTO.Events;

public record GetEventsFilters(
    IEnumerable<ParticipantsType?>? Participants = null,
    IEnumerable<TimeType?>? Time = null,
    IEnumerable<PriceType?>? Price = null,
    string? EventName = null,
    string? OrganizerName = null,
    bool OnlyAvailablePlace = false,
    bool OrganizedByMe = false);

public enum ParticipantsType
{
    To50,
    From50To100,
    Above100
}

public enum TimeType
{
    Past,
    Current,
    Future
}

public enum PriceType
{
    Free,
    Paid
}