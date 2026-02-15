using Diplomski.RatingHub.Application.Interfaces.Models;

namespace Diplomski.RatingHub.Application.Interfaces.Builders;

public interface IPaginatedListBuilder<T>
{
    public IPaginatedListBuilder<T> WithSkip(int skip);
    public IPaginatedListBuilder<T> WithTake(int take);
    public IPaginatedListBuilder<T> WithTotalCount(int totalCount);
    public IPaginatedListBuilder<T> WithItems(ICollection<T> items);
    IPaginatedList<T> Build();
}