namespace Catalog.API.Models;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class PagedQueryParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // optional
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool Descending { get; set; } = false;
}