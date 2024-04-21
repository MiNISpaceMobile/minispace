namespace Api.DTO.Paging;

public record SortRequest(
    IEnumerable<string> SortBy);
