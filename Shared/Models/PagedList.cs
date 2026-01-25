namespace Shared.Models;

public class PagedList<T>
{
    public List<T> Data { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public class PagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12; // Good for 3x4 grid
    public string? SearchTerm { get; set; } = string.Empty;
    public string? CardId { get; set; } = string.Empty;
}