namespace Api.DTO.Paging;

public record Pageable(
    int PageNumber,
    int PageSize,
    Sort Sort,
    int Offset,
    bool Paged,
    bool Unpaged);
