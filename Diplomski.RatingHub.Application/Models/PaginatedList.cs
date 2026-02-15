using Diplomski.RatingHub.Application.Interfaces.Models;

namespace Diplomski.RatingHub.Application.Models;

public class PaginatedList<TPaginatedListItem> : IPaginatedList<TPaginatedListItem>
{
    public IReadOnlyCollection<TPaginatedListItem> Items { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }

    public int TotalCount { get; set; }

    public bool HasPreviousPage { get; set; }

    public bool HasNextPage { get; set; }
}