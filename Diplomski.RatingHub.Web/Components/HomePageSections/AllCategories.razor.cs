using Microsoft.AspNetCore.Components;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.HomePageSections;

public partial class AllCategories : ComponentBase
{
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    [Parameter] public IEnumerable<CategoryTreeItem> Categories { get; set; } = [];

    protected void OnExpand(TreeExpandEventArgs args)
    {
        // Ovde kasnije možeš da dohvatiš children iz baze kada se čvor proširi.
        // Za sada koristimo postojeće Children podatke.
        var category = args.Value as CategoryTreeItem;

        args.Children.Data = category.Children;
        args.Children.TextProperty = "Name";
        args.Children.HasChildren = (category) => (category as CategoryTreeItem).HasChildren;
    }

    protected void OnChange(TreeEventArgs args)
    {
        if (args.Value is CategoryTreeItem node && !string.IsNullOrWhiteSpace(node.Slug))
        {
            Navigation.NavigateTo($"/categories/{node.Slug}");
        }
    }

    public class CategoryTreeItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public List<CategoryTreeItem> Children { get; set; } = [];
        public bool HasChildren => Children.Count > 0;
    }
}