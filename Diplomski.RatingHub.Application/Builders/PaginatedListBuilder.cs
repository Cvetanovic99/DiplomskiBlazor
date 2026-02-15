using Diplomski.RatingHub.Application.Interfaces.Builders;
using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;

namespace Diplomski.RatingHub.Application.Builders;

public class PaginatedListBuilder<T> : IPaginatedListBuilder<T>
{
    private PaginatedList<T> _output;
    private bool _totalCountDefined;

    public PaginatedListBuilder()
    {
        _output = new PaginatedList<T>();
    }

    public IPaginatedListBuilder<T> WithSkip(int skip)
    {
        _output.Skip = skip;
        return this;
    }

    public IPaginatedListBuilder<T> WithTake(int take)
    {
        _output.Take = take;
        return this;
    }

    public IPaginatedListBuilder<T> WithTotalCount(int totalCount)
    {
        _totalCountDefined = true;
        _output.TotalCount = totalCount;
        return this;
    }

    public IPaginatedListBuilder<T> WithItems(ICollection<T> items)
    {
        _output.Items = items.ToList().AsReadOnly();
        return this;
    }

    public IPaginatedList<T> Build()
    {
        if (_output.Items == null)
            throw new ArgumentException("Items is required for PaginatedList!");

        if (!_totalCountDefined)
            throw new ArgumentException("TotalCount is required for PaginatedList!");

        if (_output.Take > 0)
        {
            _output.TotalPages = (int)Math.Ceiling(_output.TotalCount / (double)_output.Take);
            _output.PageNumber = (_output.Skip / _output.Take) + 1;
        }
        else
        {
            _output.TotalPages = 1;
            _output.PageNumber = 1;
        }

        _output.HasPreviousPage = _output.PageNumber > 1;
        _output.HasNextPage = _output.PageNumber < _output.TotalPages;

        var output = _output;
        _output = new PaginatedList<T>();
        return output;
    }
}