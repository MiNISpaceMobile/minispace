namespace Api.DTO;

public record OrganizingUnitDto(long Id, string Name, long? ParentId);

public record OrganizingUnitSearchResponse(long Id, string Name, long? ParentId)
{
    public bool isLeaf => ParentId == null;
}


