using Diplomski.RatingHub.Domain.Enums;

namespace Diplomski.RatingHub.Web.Components.AdminPages.CategoryPages;

public static class NewCategorySuggestionsFilterValues
{
    public static readonly IEnumerable<NewCategorySuggestionStatus> CategorySuggestionStatuses =
        Enum.GetValues<NewCategorySuggestionStatus>();
}