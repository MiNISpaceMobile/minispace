namespace Api.DTO.OrganizingUnits;

public record OrganizingUnitSearchResponse(
    long Id,
    string Name,
    long? ParentId,
    bool IsLeaf);