using Api.DTO.OrganizingUnits;

namespace Api.DTO.Events;

public record CreateEvent(
    long Id,
    Guid Uuid,
    string Name,
    OrganizingUnitDto OrganizingUnit,
    DateTime Date,
    string Description);
