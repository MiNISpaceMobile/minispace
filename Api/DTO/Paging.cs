namespace Api.DTO;

public record PageableRequest(int Page, int Size, SortRequest Sort);

public record SortRequest(IEnumerable<string> SortBy);

public record Sort(bool Empty, bool Sorted, bool Unsorted);

public record Pageable(int PageNumber, int PageSize, Sort Sort, int Offset, bool Paged, bool Unpaged);

// I think there might be something wrong with it but it is identical as APISpec says.
public class PagedResponse<T>
{
    public IEnumerable<T> Content { get; set; }
    public Pageable Pageable { get; set; }
    public bool First {  get; set; }
    public bool Last {  get; set; }
    public bool Empty { get; set; }
    public int TotalPages {  get; set; }
    public int TotalElements {  get; set; }
    public int Size {  get; set; }
    public int Number {  get; set; }
    public int NumberOfElements { get; set; }
#pragma warning disable CS8618
    public PagedResponse() { }
#pragma warning restore CS8618
}