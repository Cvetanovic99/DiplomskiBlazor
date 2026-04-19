using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.HomePageSections;

public partial class MoreCategories : ComponentBase
{
    [Parameter] public IEnumerable<CategoryItem> Categories { get; set; } = [];

    public class CategoryItem
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }
}