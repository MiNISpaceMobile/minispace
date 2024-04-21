namespace Api.DTO.Paging;

public record PagedResponse<T>(
    IEnumerable<T> Content,
    Pageable Pageable,
    bool First,
    bool Last,
    bool Empty,
    int TotalPages,
    int TotalElements,
    int Size,
    int Number,
    int NumberOfElements);