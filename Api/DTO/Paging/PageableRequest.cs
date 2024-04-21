namespace Api.DTO.Paging;

public record PageableRequest(
    int Page,
    int Size,
    SortRequest Sort);
