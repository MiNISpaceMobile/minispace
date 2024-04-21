namespace Api.DTO.OrganizingUnits;

public record OrganizingUnitDto(
    long Id,
    string Name,
    long? ParentId);
