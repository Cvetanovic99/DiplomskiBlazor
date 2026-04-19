using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.HomePageSections;

public partial class MainCategoriesSection : ComponentBase
{
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    protected bool ShowMoreCategories { get; set; }
    protected bool ShowAllCategories { get; set; }

    protected List<CategoryCardItem> MainCategories { get; set; } = [];
    protected List<MoreCategories.CategoryItem> MoreCategoriesList { get; set; } = [];
    protected List<AllCategories.CategoryTreeItem> AllCategoriesTree { get; set; } = [];

    protected override void OnInitialized()
    {
        MainCategories =
        [
            new("Frizeri", "content_cut", "frizeri"),
            new("Kozmetički saloni", "spa", "kozmeticki-saloni"),
            new("Vodoinstalateri", "plumbing", "vodoinstalateri"),
            new("Električari", "bolt", "elektricari"),
            new("Auto servisi", "directions_car", "auto-servisi"),
            new("Zubari", "medical_services", "zubari"),
            new("Advokati", "gavel", "advokati"),
            new("Računovođe", "calculate", "racunovodje"),
            new("Servis bele tehnike", "kitchen", "servis-bele-tehnike"),
            new("Krečenje", "format_paint", "krecenje"),
            new("Fotografi", "photo_camera", "fotografi"),
            new("Teretane", "fitness_center", "teretane")
        ];

        MoreCategoriesList =
        [
            new MoreCategories.CategoryItem { Name = "Klima uređaji", Icon = "ac_unit", Slug = "klima-uredjaji" },
            new MoreCategories.CategoryItem { Name = "Stolarija", Icon = "construction", Slug = "stolarija" },
            new MoreCategories.CategoryItem { Name = "Bravari", Icon = "hardware", Slug = "bravari" },
            new MoreCategories.CategoryItem { Name = "Selidbe", Icon = "local_shipping", Slug = "selidbe" },
            new MoreCategories.CategoryItem { Name = "Čišćenje", Icon = "cleaning_services", Slug = "ciscenje" },
            new MoreCategories.CategoryItem { Name = "Moleraji", Icon = "format_color_fill", Slug = "moleraji" },
            new MoreCategories.CategoryItem { Name = "Keramičari", Icon = "grid_view", Slug = "keramicari" },
            new MoreCategories.CategoryItem { Name = "IT servis", Icon = "computer", Slug = "it-servis" },
            new MoreCategories.CategoryItem { Name = "Veterinari", Icon = "pets", Slug = "veterinari" },
            new MoreCategories.CategoryItem { Name = "Cvećare", Icon = "local_florist", Slug = "cvecare" },
            new MoreCategories.CategoryItem { Name = "Catering", Icon = "restaurant", Slug = "catering" },
            new MoreCategories.CategoryItem { Name = "Nameštaj", Icon = "chair", Slug = "namestaj" }
        ];

        AllCategoriesTree =
        [
            new AllCategories.CategoryTreeItem
            {
                Id = "lepota-i-nega",
                Name = "Lepota i nega",
                Slug = "lepota-i-nega",
                // [
                //     new AllCategories.CategoryTreeItem { Id = "frizeri", Name = "Frizeri", Slug = "frizeri" },
                //     new AllCategories.CategoryTreeItem { Id = "muski-frizeri", Name = "Muški frizeri", Slug = "muski-frizeri" },
                //     new AllCategories.CategoryTreeItem { Id = "kozmeticki-saloni", Name = "Kozmetički saloni", Slug = "kozmeticki-saloni" },
                //     new AllCategories.CategoryTreeItem { Id = "sminkeri", Name = "Šminkeri", Slug = "sminkeri" }
                // ]
            },
            new AllCategories.CategoryTreeItem
            {
                Id = "kucne-usluge",
                Name = "Kućne usluge",
                Slug = "kucne-usluge",
                Children =
                [
                    new AllCategories.CategoryTreeItem { Id = "vodoinstalateri", Name = "Vodoinstalateri", Slug = "vodoinstalateri" },
                    new AllCategories.CategoryTreeItem { Id = "elektricari", Name = "Električari", Slug = "elektricari" },
                    new AllCategories.CategoryTreeItem { Id = "krecenje", Name = "Krečenje", Slug = "krecenje" },
                    new AllCategories.CategoryTreeItem { Id = "keramicari", Name = "Keramičari", Slug = "keramicari" },
                    new AllCategories.CategoryTreeItem { Id = "stolarija", Name = "Stolarija", Slug = "stolarija" }
                ]
            },
            new AllCategories.CategoryTreeItem
            {
                Id = "automobili",
                Name = "Automobili",
                Slug = "automobili",
                Children =
                [
                    new AllCategories.CategoryTreeItem { Id = "auto-servisi", Name = "Auto servisi", Slug = "auto-servisi" },
                    new AllCategories.CategoryTreeItem { Id = "auto-elektrika", Name = "Auto elektrika", Slug = "auto-elektrika" },
                    new AllCategories.CategoryTreeItem { Id = "vulkanizeri", Name = "Vulkanizeri", Slug = "vulkanizeri" }
                ]
            },
            new AllCategories.CategoryTreeItem
            {
                Id = "zdravlje-i-strucne-usluge",
                Name = "Zdravlje i stručne usluge",
                Slug = "zdravlje-i-strucne-usluge",
                Children =
                [
                    new AllCategories.CategoryTreeItem { Id = "zubari", Name = "Zubari", Slug = "zubari" },
                    new AllCategories.CategoryTreeItem { Id = "advokati", Name = "Advokati", Slug = "advokati" },
                    new AllCategories.CategoryTreeItem
                    {
                        Id = "racunovodje", Name = "Računovođe", Slug = "racunovodje", 
                        Children = [
                            new AllCategories.CategoryTreeItem { Id = "frizeri", Name = "Frizeri", Slug = "frizeri" },
                            new AllCategories.CategoryTreeItem { Id = "muski-frizeri", Name = "Muški frizeri", Slug = "muski-frizeri" },
                            new AllCategories.CategoryTreeItem { Id = "kozmeticki-saloni", Name = "Kozmetički saloni", Slug = "kozmeticki-saloni" },
                            new AllCategories.CategoryTreeItem
                            {
                                Id = "sminkeri", Name = "Šminkeri", Slug = "sminkeri", 
                                Children = [
                                    new AllCategories.CategoryTreeItem { Id = "auto-servisi", Name = "Auto servisi", Slug = "auto-servisi" },
                                    new AllCategories.CategoryTreeItem { Id = "auto-elektrika", Name = "Auto elektrika", Slug = "auto-elektrika" },
                                    new AllCategories.CategoryTreeItem { Id = "vulkanizeri", Name = "Vulkanizeri", Slug = "vulkanizeri" }
                                ]
                            }
                        ]
                    }
                ]
            }
        ];
    }

    protected void ToggleMoreCategories()
    {
        ShowMoreCategories = !ShowMoreCategories;
    }

    protected void ToggleAllCategories()
    {
        ShowAllCategories = !ShowAllCategories;
    }

    protected string GetCategoryUrl(CategoryCardItem category)
    {
        return $"/categories/{category.Slug}";
    }

    protected class CategoryCardItem(string name, string icon, string slug)
    {
        public string Name { get; set; } = name;
        public string Icon { get; set; } = icon;
        public string Slug { get; set; } = slug;
    }
}