namespace Api.DTO;

public record PageableRequest(
    int Page, 
    int Size, 
    SortRequest Sort);

public record SortRequest(
    IEnumerable<string> SortBy);

public record Sort(
    bool Empty, 
    bool Sorted, 
    bool Unsorted);

public record Pageable(
    int PageNumber, 
    int PageSize, 
    Sort Sort, 
    int Offset, 
    bool Paged, 
    bool Unpaged);

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