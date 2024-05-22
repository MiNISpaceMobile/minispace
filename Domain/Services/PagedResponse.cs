using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services;

public class PagedResponse<T>
{
    public IEnumerable<T> Items { get; set; }
    public int Count => Items.Count();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool IsLast { get; set; }
    public PagedResponse(IQueryable<T> items, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = items.Count();
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
        IsLast = PageIndex + 1 >= TotalPages;
        Items = [.. items.Skip(PageIndex * PageSize).Take(PageSize)];
    }

    public PagedResponse() 
    {
        Items = [];
    }
}
