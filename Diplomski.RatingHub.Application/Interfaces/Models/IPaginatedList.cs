namespace Diplomski.RatingHub.Application.Interfaces.Models;

public interface IPaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int Skip { get; }
    public int Take { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }
}