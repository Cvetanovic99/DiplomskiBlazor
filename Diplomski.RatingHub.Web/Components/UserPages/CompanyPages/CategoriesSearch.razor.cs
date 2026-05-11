using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Application.UseCases.Cities.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyPages;

public partial class CategoriesSearch
{
    [Inject] public ICityDataService CityDataService { get; set; } = null!;
    [Inject] public ICategoryDataService CategoryDataService { get; set; } = null!;
    
    private SearchDto Model = new();

    private IEnumerable<CityDto> _cities = new List<CityDto>();
    private string _selectedCityText;

    private IEnumerable<CategoryWithBreadCrumbDto> _categories = new List<CategoryWithBreadCrumbDto>();
    private string _selectedCategoryText;
    

    private async Task LoadCities(LoadDataArgs args)
    {
        var response = await InvokeDataServiceMethod(
            () => CityDataService.GetCities(
                args.Filter?.ToLower() ?? string.Empty,
                new QueryArgs { Take = 10, Skip = 0 }),
            errorMessage: "Greška prilikom ucitavanja gradova");

        if(!response.ExceptionOccurred)
            _cities = response.Result.Items;
    }

    private void OnCityChanged(object value)
    {
        var text = value?.ToString();

        if (string.IsNullOrWhiteSpace(text))
        {
            _selectedCityText = null;
            Model.CityId = 0;
            return;
        }

        _selectedCityText = text;

        var selectedCity = _cities
            .FirstOrDefault(x => string.Equals(x.Name, text, StringComparison.CurrentCultureIgnoreCase));

        Model.CityId = selectedCity?.Id ?? 0;
        StateHasChanged();
    }

    private void OnCitySelected()
    {
        StateHasChanged();
    }

    private async Task LoadCategories(LoadDataArgs args)
    {
        var res = await InvokeDataServiceMethod(
            () => CategoryDataService.GetCategoriesWithBreadCrumb(args.Filter?.ToLower() ?? string.Empty, 10),
            errorMessage: "Greška prilikom ucitavanja kategorija");

        if (!res.ExceptionOccurred)
            _categories = res.Result?.ToList();
    }

    private void OnCategoryChanged(object value)
    {
        var text = value?.ToString();

        if (string.IsNullOrWhiteSpace(text))
        {
            _selectedCategoryText = null;
            Model.CategoryId = 0;
            return;
        }

        _selectedCategoryText = text;

        var selectedCategory = _categories
            .FirstOrDefault(x =>string.Equals(x.Name, text, StringComparison.CurrentCultureIgnoreCase));

        Model.CategoryId = selectedCategory?.Id ?? 0;
        StateHasChanged();
    }
    
    private void OnCategorySelected()
    {
        StateHasChanged();
    }

    private void OnSearchClicked()
    {

        if (Model.CityId == 0 || Model.CategoryId == 0)
            return;
        
        NavigationManager.NavigateTo($"/companies?CityId={Model.CityId}&CategoryId={Model.CategoryId}", true);
    }

    private bool ValidateCategory()
    {
        return Model.CategoryId != 0;
    }

    private bool ValidateCity()
    {
        return Model.CityId != 0;
    }

    public class SearchDto
    {
        public int CityId { get; set; }
        public int CategoryId { get; set; }
    }
}