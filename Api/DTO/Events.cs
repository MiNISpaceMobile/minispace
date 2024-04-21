namespace Api.DTO;

public record EventDto(long Id, Guid Uuid, string Name, OrganizingUnitDto OrganizingUnit, 
                       DateTime Date, string Description, int ParticipantCount);

public record CreateEvent(long Id, Guid Uuid, string Name, OrganizingUnitDto OrganizingUnit,
                          DateTime Date, string Description);

public record EventSearchDetails(string Name, string Organizer, DateTime DateFrom, DateTime DateTo,
                                 PageableRequest Pageable);
