using Api.DTO.Paging;

namespace Api.DTO.Events;

public record EventSearchDetails(
    string Name,
    string Organizer,
    DateTime DateFrom,
    DateTime DateTo,
    PageableRequest Pageable);