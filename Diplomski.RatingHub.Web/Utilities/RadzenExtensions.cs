using Diplomski.RatingHub.Application.Models;
using Radzen;

namespace Diplomski.RatingHub.Web.Utilities;

public static class RadzenExtensions
{
    public static QueryArgs ToQueryArgs(this LoadDataArgs loadDataArgs)
    {
        return new QueryArgs
        {
            Filter = loadDataArgs.Filter,
            OrderBy = loadDataArgs.OrderBy,
            Skip = loadDataArgs.Skip,
            Take = loadDataArgs.Top
        };
    }
}