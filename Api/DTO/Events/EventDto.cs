using Api.DTO.OrganizingUnits;

namespace Api.DTO.Events;

public record EventDto(
    long Id,
    Guid Uuid,
    string Name,
    OrganizingUnitDto OrganizingUnit,
    DateTime Date,
    string Description,
    int ParticipantCount);
