using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.HomePageSections;

public partial class MainCategoriesSection
{
    [Inject] public ICategoryDataService CategoryDataService { get; set; }

    protected bool ShowMoreCategories { get; set; }
    protected bool ShowAllCategories { get; set; }

    protected List<TopCategoryDto> _mainCategories { get; set; } = new();
    protected List<TopCategoryDto> _moreCategoriesList { get; set; } = new();
    protected List<SubcategoryDto> _allCategoriesTree { get; set; } = new();
    //protected List<AllCategories.CategoryTreeItem> AllCategoriesTree { get; set; } = [];

    private int _mainCategoriesShowNumber = 0;

    protected override async Task OnInitializedAsync()
    {
        var response = await InvokeDataServiceMethod(
            () => CategoryDataService.GetAllTopCategories(),
            errorMessage: "Greška prilikom ucitavanja kategorija");

        if (!response.ExceptionOccurred)
        {
            _mainCategories = response.Result.ToList();
            _moreCategoriesList = _mainCategories.Skip(_mainCategoriesShowNumber).ToList();
            _allCategoriesTree = _mainCategories.Select(c => 
                new SubcategoryDto {Id = c.Id, Name = c.Name, HasChildren = c.HasChildren}).ToList();
        }
    }

    private void ToggleMoreCategories()
    {
        ShowMoreCategories = !ShowMoreCategories;
    }

    private void ToggleAllCategories()
    {
        ShowAllCategories = !ShowAllCategories;
    }

    private async Task OnCategoryClick(TopCategoryDto category)
    {
        var result = await DialogService.OpenAsync<SelectCityDialog>(
            "Izbor grada",
            options: new DialogOptions
            {
                Width = "500px",
                Height = "auto",
                Style = "margin-top: 130px"
            });

        if (result != null)
        {
            int cityId = (int)result;
            NavigationManager.NavigateTo($"/companies?CityId={cityId}&CategoryId={category.Id}");
        }
    }
}